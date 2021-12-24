using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using BadScript.Exceptions;
using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Reflection;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

using Ceen.Httpd;
using Ceen.Httpd.Logging;

using OpenSSL.PrivateKeyDecoder;

namespace BadScript.HttpServer
{

    public class BSHttpServerInterface : ABSScriptInterface
    {

        #region Public

        public BSHttpServerInterface() : base( "HttpServer" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(
                               new BSObject( "CreateListener" ),
                               new BSFunction( "function CreateListener(config)", CreateListener, 1 )
                              );

            BSTable statusCodes = EnumBuilder < HttpStatusCode >.Build();
            statusCodes.Lock();

            root.InsertElement(
                               new BSObject( "StatusCodes" ),
                               statusCodes
                              );
        }

        #endregion

        #region Private

        private ABSObject CreateListener( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is ABSTable configTable )
            {
                CancellationTokenSource tcs = new CancellationTokenSource();
                ServerConfig config = new ServerConfig().AddLogger( new CLFStdOut() );

                string endpoint = configTable.GetRawElement( new BSObject( "ip" ) ).ConvertString();
                int port = int.Parse( configTable.GetRawElement( new BSObject( "port" ) ).ConvertString() );

                if ( configTable.HasElement( new BSObject( "routes" ) ) &&
                     configTable.GetRawElement( new BSObject( "routes" ) ) is IEnumerable < IForEachIteration > routes )
                {
                    foreach ( IForEachIteration route in routes )
                    {
                        ABSObject[] vals = route.GetObjects();
                        config = config.AddRoute( vals[0].ConvertString(), new BSFunctionHandler( vals[1] ) );
                    }
                }

                bool usessl = false;

                if ( configTable.HasElement( new BSObject( "ssl" ) ) )
                {
                    usessl = true;
                    ABSTable sslConfig = ( ABSTable )configTable.GetRawElement( new BSObject( "ssl" ) );

                    string sslFile = sslConfig.GetRawElement( new BSObject( "cert-file" ) ).ConvertString();

                    if ( !File.Exists( sslFile ) )
                    {
                        throw new BSRuntimeException( $"SSL Certificate not Found: '{sslFile}'" );
                    }

                    if ( sslConfig.HasElement( new BSObject( "cert-pass" ) ) )
                    {
                        string pw = sslConfig.GetRawElement( new BSObject( "cert-pass" ) ).ConvertString();

                        config.SSLCertificate =
                            new X509Certificate2(
                                                 sslFile,
                                                 pw
                                                );
                    }
                    else
                    {
                        config.SSLCertificate =
                            new X509Certificate2(
                                                 sslFile
                                                );
                    }

                    if ( sslConfig.HasElement( new BSObject( "cert-key" ) ) )
                    {
                        string certKeyFile = sslConfig.GetRawElement( new BSObject( "cert-key" ) ).ConvertString();
                        IOpenSSLPrivateKeyDecoder decoder = new OpenSSLPrivateKeyDecoder();

                        config.SSLCertificate =
                            ( config.SSLCertificate as X509Certificate2 ).CopyWithPrivateKey(
                                 decoder.Decode(
                                                File.ReadAllText( certKeyFile )
                                               )
                                );
                    }
                }

                return new HttpServerListenerObject(
                                                    SourcePosition.Unknown,
                                                    new IPEndPoint( IPAddress.Parse( endpoint ), port ),
                                                    usessl,
                                                    config,
                                                    tcs
                                                   );
            }

            throw new BSRuntimeException( "Invalid HTTP Server Configuration" );
        }

        #endregion

    }

}

using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using Ceen.Httpd;
using Ceen.Httpd.Logging;

namespace BadScript.Https
{
    public class HttpServerApi : ABSScriptInterface
    {
        public HttpServerApi() : base("http-server")
        {
        }

        public override void AddApi(ABSTable root)
        {
            root.InsertElement(new BSObject("createListener"), new BSFunction("function createListener(config)", CreateListener, 1));
        }

        
        
        private ABSObject CreateListener(ABSObject[] arg)
        {
            if (arg[0] is ABSTable configTable)
            {
                var tcs = new CancellationTokenSource();
                var config = new ServerConfig().AddLogger(new CLFStdOut());

                string endpoint = configTable.GetRawElement(new BSObject("ip")).ConvertString();
                int port = int.Parse(configTable.GetRawElement(new BSObject("port")).ConvertString());
                
                if (configTable.HasElement(new BSObject("routes")) &&
                    configTable.GetRawElement(new BSObject("routes")) is BSTable routes)
                {
                    foreach (var route in routes)
                    {
                        ABSObject[] vals = route.GetObjects();
                        config = config.AddRoute(vals[0].ConvertString(), new BSFunctionHandler(vals[1]));
                    }
                }

                var usessl = false;

                if (configTable.HasElement(new BSObject("ssl")))
                {
                    usessl = true;
                    ABSTable sslConfig = (ABSTable) configTable.GetRawElement(new BSObject("ssl"));
                    string pw = sslConfig.GetRawElement(new BSObject("cert-pass")).ConvertString();
                    config.SSLCertificate =
                        new X509Certificate2(sslConfig.GetRawElement(new BSObject("cert-file")).ConvertString(), pw);
                }

                return new HttpServerListenerObject(SourcePosition.Unknown,
                    new IPEndPoint(IPAddress.Parse(endpoint), port), usessl, config, tcs);
            }

            throw new BSRuntimeException("Invalid HTTP Server Configuration");
        }
    }
}
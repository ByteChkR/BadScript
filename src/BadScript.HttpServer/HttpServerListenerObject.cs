using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using Ceen.Httpd;

namespace BadScript.HttpServer
{

    public class HttpServerListenerObject : ABSObject
    {
        private BSTable m_InstanceFunctions;
        private Task m_Listener;
        private CancellationTokenSource m_TokenSource;

        private ServerConfig m_Config;
        private IPEndPoint m_EndPoint;
        private bool m_UseSSL;

        public override bool IsNull => false;

        #region Public

        public HttpServerListenerObject(
            SourcePosition pos,
            IPEndPoint ep,
            bool useSsl,
            ServerConfig config,
            CancellationTokenSource src ) : base( pos )
        {
            m_TokenSource = src;
            m_Listener = null;
            m_Config = config;
            m_EndPoint = ep;
            m_UseSSL = useSsl;

            m_InstanceFunctions = new BSTable(
                pos,
                new Dictionary < ABSObject, ABSObject >
                {
                    { new BSObject( "stop" ), new BSFunction( "function stop()", StopListener, 0 ) },
                    { new BSObject( "start" ), new BSFunction( "function start()", StartListener, 0 ) },
                    {
                        new BSObject( "isRunning" ), new BSFunction(
                            "function isRunning()",
                            objects => new BSObject( ( decimal ) ( m_Listener == null ? 0 : 1 ) ),
                            0 )
                    }
                }
            );
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return m_InstanceFunctions.GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_InstanceFunctions.HasProperty( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( Position, "HTTP Server Objects can not be invoked" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return doneList[this] = m_Listener?.ToString() ?? "NULL(HTTP Server)";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, "HTTP Server Objects can not be written to" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = m_Listener == null;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = $"HTTP Server('{m_Listener}')";

            return false;
        }

        #endregion

        #region Private

        private ABSObject StartListener( ABSObject[] arg )
        {
            if ( m_Listener == null )
            {
                m_Listener = Ceen.Httpd.HttpServer.ListenAsync( m_EndPoint, m_UseSSL, m_Config, m_TokenSource.Token );
            }
            else
            {
                throw new BSRuntimeException( "HTTP Server is already running" );
            }

            return new BSObject( null );
        }

        private ABSObject StopListener( ABSObject[] arg )
        {
            if ( m_Listener != null )
            {
                m_TokenSource.Cancel();
                m_Listener.Wait();
                m_Listener = null;
            }
            else
            {
                throw new BSRuntimeException( "HTTP Server is not running" );
            }

            return new BSObject( null );
        }

        #endregion
    }

}

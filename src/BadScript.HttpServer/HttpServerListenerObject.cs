using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Reflection;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

using Ceen.Httpd;

namespace BadScript.HttpServer
{

    public class HttpServerListenerObject : ABSObject
    {

        private readonly BSTable m_InstanceFunctions;
        private Task m_Listener;
        private readonly CancellationTokenSource m_TokenSource;

        private readonly ServerConfig m_Config;
        private readonly IPEndPoint m_EndPoint;
        private readonly bool m_UseSSL;

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
                                                  {
                                                      new BSObject( "Stop" ),
                                                      new BSFunction( "function Stop()", args => StopListener(), 0 )
                                                  },
                                                  {
                                                      new BSObject( "Start" ),
                                                      new BSFunction( "function Start()", args => StartListener(), 0 )
                                                  },
                                                  {
                                                      new BSObject( "IsRunning" ), new BSReflectionReference(() => m_Listener == null ? BSObject.False : BSObject.True, null)
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

        private ABSObject StartListener()
        {
            if ( m_Listener == null )
            {
                m_Listener = Ceen.Httpd.HttpServer.ListenAsync( m_EndPoint, m_UseSSL, m_Config, m_TokenSource.Token );
            }
            else
            {
                throw new BSRuntimeException( "HTTP Server is already running" );
            }

            return BSObject.Null;
        }

        private ABSObject StopListener()
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

            return BSObject.Null;
        }

        #endregion

    }

}

using System.Collections.Generic;
using System.Net;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Http
{
    public class HttpListenerObject : ABSObject
    {
        
        private BSTable m_InstanceFunctions;
        private HttpListener m_Listener;

        public override bool IsNull => false;

        #region Public

        public HttpListenerObject( SourcePosition pos, HttpListener listener) : base( pos )
        {
            m_Listener = listener;

            m_InstanceFunctions = new BSTable(
                SourcePosition.Unknown,
                new Dictionary < ABSObject, ABSObject >
                {
                    {
                        new BSObject( "close" ), new BSFunction(
                            "function close()", CloseListener,
                            0
                        )
                    },
                    {
                        new BSObject("start"),
                        new BSFunction("function start()", StartListener,0 )
                    },
                    {
                        new BSObject("stop"),
                        new BSFunction("function stop()", StopListener,0 )
                    },
                    {
                        new BSObject("addPrefixes"),
                        new BSFunction("function addPrefixes(prefixes)", ListenerAddPrefixes, 1 )
                    },
                    {
                        new BSObject("clearPrefixes"),
                        new BSFunction("function clearPrefixes()", ListenerClearPrefixes, 0 )
                    },
                    {
                        new BSObject("getContext"),
                        new BSFunction("function getContext()", ListenerGetContext, 0 )
                    }
                }
            );
        }

        private ABSObject ListenerGetContext(ABSObject[] arg)
        {
            HttpListenerContext c = m_Listener.GetContext();
            return new HttpListenerContextObject(SourcePosition.Unknown, c);
        }

        private ABSObject ListenerClearPrefixes(ABSObject[] arg)
        {
            m_Listener.Prefixes.Clear();
            return new BSObject(null);
        }

        private ABSObject ListenerAddPrefixes(ABSObject[] arg)
        {
            if (arg[0] is ABSArray a)
            {
                for (int i = 0; i < a.GetLength(); i++)
                {
                    m_Listener.Prefixes.Add(a.GetRawElement(i).ConvertString());
                }
                return new BSObject(null);
            }

            throw new BSInvalidTypeException(arg[0].Position, "Expected Array of Prefixes", arg[0], "Array");
        }

        private ABSObject StartListener(ABSObject[] arg)
        {
            m_Listener.Start();
            return new BSObject(null);
        }
        private ABSObject StopListener(ABSObject[] arg)
        {
            m_Listener.Stop();
            return new BSObject(null);
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
            throw new BSRuntimeException( Position, "HTTPListener Objects can not be invoked" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return doneList[this] = m_Listener?.ToString() ?? "NULL(HTTPListener)";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, "HTTPListener Objects can not be written to" );
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
            v = $"HTTPListener('{m_Listener}')";

            return false;
        }

        #endregion

        #region Private

        private ABSObject CloseListener( ABSObject[] arg )
        {
            if ( m_Listener == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            m_Listener.Close();
            m_Listener = null;

            return new BSObject( 0 );
        }

        #endregion
    }
}
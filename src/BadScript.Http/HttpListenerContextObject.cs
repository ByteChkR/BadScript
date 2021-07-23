using System.Collections.Generic;
using System.Net;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Http
{
    public class HttpListenerContextObject : ABSObject
    {
        private BSTable m_InstanceFunctions;
        private HttpListenerContext m_Context;

        public override bool IsNull => false;

        #region Public

        public HttpListenerContextObject( SourcePosition pos, HttpListenerContext context) : base( pos )
        {
            m_Context = context;

            m_InstanceFunctions = new BSTable(
                SourcePosition.Unknown,
                new Dictionary < ABSObject, ABSObject >
                {
                    {new BSObject("request"), new BSFunction("function request()", ContextGetRequest,0 )},
                    {new BSObject("response"), new BSFunction("function response()", ContextGetResponse,0 )}
                }
            );
        }

        private ABSObject ContextGetRequest(ABSObject[] arg)
        {
            HttpListenerRequest r = m_Context.Request;
            return new HttpListenerRequestObject(SourcePosition.Unknown, r);
        }
        private ABSObject ContextGetResponse(ABSObject[] arg)
        {
            HttpListenerResponse r = m_Context.Response;
            return new HttpListenerResponseObject(SourcePosition.Unknown,  r);
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
            throw new BSRuntimeException( Position, "HTTPListenerContext Objects can not be invoked" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return doneList[this] = m_Context?.ToString() ?? "NULL(HTTPListenerContext)";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, "HTTPListenerContext Objects can not be written to" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = m_Context == null;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = $"HTTPListenerContext('{m_Context}')";

            return false;
        }

        #endregion

    }
}
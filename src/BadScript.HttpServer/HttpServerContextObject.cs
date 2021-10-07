using System.Collections.Generic;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

using Ceen;

namespace BadScript.HttpServer
{

    public class HttpServerContextObject : ABSObject
    {

        private readonly BSTable m_InstanceFunctions;
        private readonly IHttpContext m_Context;

        #region Public

        public HttpServerContextObject( SourcePosition pos, IHttpContext context ) : base( pos )
        {
            m_Context = context;

            m_InstanceFunctions = new BSTable(
                                              pos,
                                              new Dictionary < ABSObject, ABSObject >
                                              {
                                                  {
                                                      new BSObject( "Request" ),
                                                      new BSFunction( "function Request()", ContextGetRequest, 0 )
                                                  },
                                                  {
                                                      new BSObject( "Response" ),
                                                      new BSFunction( "function Response()", ContextGetResponse, 0 )
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
            throw new BSRuntimeException( Position, "HTTPListenerContext Objects can not be invoked" );
        }

        public override bool IsNull()
        {
            return false;
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

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_Context.GetHashCode() ^ m_InstanceFunctions.GetHashCode();
        }

        #endregion

        #region Private

        private ABSObject ContextGetRequest( ABSObject[] arg )
        {
            IHttpRequest r = m_Context.Request;

            return new HttpServerRequestObject( SourcePosition.Unknown, r );
        }

        private ABSObject ContextGetResponse( ABSObject[] arg )
        {
            IHttpResponse r = m_Context.Response;

            return new HttpServerResponseObject( SourcePosition.Unknown, r );
        }

        #endregion

    }

}

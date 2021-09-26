using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

using Ceen;

namespace BadScript.HttpServer
{

    public class HttpServerRequestObject : ABSObject
    {

        private BSTable m_InstanceFunctions;
        private IHttpRequest m_Request;

        public override bool IsNull => false;

        #region Public

        public HttpServerRequestObject( SourcePosition pos, IHttpRequest request ) : base( pos )
        {
            m_Request = request;

            Dictionary < ABSObject, ABSObject > headers = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, string > header in m_Request.Headers )
            {
                headers[new BSObject( header.Key )] = new BSObject( m_Request.Headers[header.Value] );
            }

            Dictionary < ABSObject, ABSObject > queryTable = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, string > s in m_Request.QueryString )
            {
                queryTable[new BSObject( s.Key )] = new BSObject( s.Value );
            }

            m_InstanceFunctions = new BSTable(
                                              pos,
                                              new Dictionary < ABSObject, ABSObject >
                                              {
                                                  { new BSObject( "headers" ), new BSTable( pos, headers ) },
                                                  {
                                                      new BSObject( "uri" ),
                                                      new BSObject( m_Request.RawHttpRequestLine.ToString() )
                                                  },
                                                  {
                                                      new BSObject( "acceptTypes" ), new BSArray(
                                                           ( m_Request.GetAcceptLanguages().
                                                                       Select( x => x.ToString() ) ??
                                                             new string[0] ).Select(
                                                                x => new BSObject( x )
                                                               )
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "contentLength" ),
                                                      new BSObject( ( decimal )m_Request.ContentLength )
                                                  },
                                                  {
                                                      new BSObject( "contentType" ),
                                                      new BSObject( m_Request.ContentType )
                                                  },
                                                  { new BSObject( "httpMethod" ), new BSObject( m_Request.Method ) },
                                                  { new BSObject( "query" ), new BSTable( pos, queryTable ) },
                                                  {
                                                      new BSObject( "readBody" ),
                                                      new BSFunction( "function readBody()", RequestReadBody, 0 )
                                                  },
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
            throw new BSRuntimeException( Position, "HTTPListenerRequest Objects can not be invoked" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return doneList[this] = m_Request?.ToString() ?? "NULL(HTTPListenerRequest)";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, "HTTPListenerRequest Objects can not be written to" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = m_Request == null;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = $"HTTPListenerRequest('{m_Request}')";

            return false;
        }

        #endregion

        #region Private

        private ABSObject RequestReadBody( ABSObject[] arg )
        {
            Encoding enc = Encoding.UTF8;

            using ( BinaryReader r = new BinaryReader( m_Request.Body ) )
            {
                return new BSObject( enc.GetString( r.ReadBytes( ( int )m_Request.ContentLength ) ) );
            }
        }

        #endregion

    }

}

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

using Ceen;

namespace BadScript.HttpServer
{

    public class HttpServerResponseObject : ABSObject
    {
        protected override int GetHashCodeImpl()
        {
            return m_Response.GetHashCode() ^ m_InstanceFunctions.GetHashCode();
        }

        private readonly BSTable m_InstanceFunctions;
        private IHttpResponse m_Response;

        public override bool IsNull() => false;

        #region Public

        public HttpServerResponseObject( SourcePosition pos, IHttpResponse response ) : base( pos )
        {
            m_Response = response;

            Dictionary < ABSObject, ABSObject > headers = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, string > header in m_Response.Headers )
            {
                headers[new BSObject( header.Key )] = new BSObject( m_Response.Headers[header.Value] );
            }

            m_InstanceFunctions = new BSTable(
                                              pos,
                                              new Dictionary < ABSObject, ABSObject >
                                              {
                                                  { new BSObject( "Headers" ), new BSTable( pos, headers ) },
                                                  {
                                                      new BSObject( "AddHeader" ), new BSFunction(
                                                           "function AddHeader(key, value)",
                                                           ResponseAddHeader,
                                                           2
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "Redirect" ),
                                                      new BSFunction( "function Redirect(url)", ResponseRedirect, 1 )
                                                  },
                                                  {
                                                      new BSObject( "WriteBody" ), new BSFunction(
                                                           "function WriteBody(bodyStr)",
                                                           ResponseWriteBody,
                                                           1
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "SetStatus" ), new BSFunction(
                                                           "function SetStatus(statusCode)/SetStatus(statusCode, message)",
                                                           SetStatusCode,
                                                           1,
                                                           2
                                                          )
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
            return doneList[this] = m_Response?.ToString() ?? "NULL(HTTPListenerRequest)";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, "HTTPListenerRequest Objects can not be written to" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = m_Response == null;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = $"HTTPListenerRequest('{m_Response}')";

            return false;
        }

        #endregion

        #region Private

        private ABSObject ResponseAddHeader( ABSObject[] arg )
        {
            m_Response.AddHeader( arg[0].ConvertString(), arg[1].ConvertString() );

            return BSObject.Null;
        }

        private ABSObject ResponseRedirect( ABSObject[] arg )
        {
            m_Response.Redirect( arg[0].ConvertString() );

            m_Response = null;

            return BSObject.Null;
        }

        private ABSObject ResponseWriteBody( ABSObject[] arg )
        {
            Encoding enc = Encoding.UTF8;
            byte[] buf = enc.GetBytes( arg[0].ConvertString() );
            Task t = m_Response.WriteAllAsync( buf );
            Task.WaitAll( t );

            return BSObject.Null;
        }

        private ABSObject SetStatusCode( ABSObject[] arg )
        {
            if ( arg.Length == 1 )
            {
                m_Response.SetStatus( ( HttpStatusCode )arg[0].ConvertDecimal() );
            }
            else
            {
                m_Response.SetStatus( ( HttpStatusCode )arg[0].ConvertDecimal(), arg[1].ConvertString() );
            }

            return BSObject.Null;
        }

        #endregion

    }

}

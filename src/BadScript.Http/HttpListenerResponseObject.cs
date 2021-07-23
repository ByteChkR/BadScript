using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Http
{
    public class HttpListenerResponseObject : ABSObject
    {
        private BSTable m_InstanceFunctions;
        private HttpListenerResponse m_Response;

        public override bool IsNull => false;

        #region Public

        public HttpListenerResponseObject( SourcePosition pos, HttpListenerResponse response) : base( pos )
        {
            m_Response = response;

            Dictionary<ABSObject, ABSObject> headers = new Dictionary<ABSObject, ABSObject>();
            foreach (string header in m_Response.Headers)
            {
                headers[new BSObject(header)] =
                    new BSArray(m_Response.Headers.GetValues(header).Select(x => new BSObject(x)));
            }

            m_InstanceFunctions = new BSTable(
                SourcePosition.Unknown,
                new Dictionary < ABSObject, ABSObject >
                {
                    {new BSObject("headers"), new BSTable(SourcePosition.Unknown, headers)},
                    {new BSObject("addHeader"), new BSFunction("function addHeader(key, value)", ResponseAddHeader, 2 )},
                    {new BSObject("redirect"), new BSFunction("function redirect(url)", ResponseRedirect, 1)},
                    {new BSObject("writeBody"), new BSFunction("function writeBody(bodyStr)", ResponseWriteBody, 1 )},
                    {new BSObject("close"), new BSFunction("function close()", ResponseClose, 0)},
                }
            );
        }

        private ABSObject ResponseClose(ABSObject[] arg)
        {
            m_Response.Close();
            m_Response = null;
            return new BSObject(null);
        }

        private ABSObject ResponseRedirect(ABSObject[] arg)
        {
            m_Response.Redirect(arg[0].ConvertString());
            m_Response.Close();
            m_Response = null;
            return new BSObject(null);
        }

        private ABSObject ResponseWriteBody(ABSObject[] arg)
        {
            Encoding enc = m_Response.ContentEncoding ?? Encoding.UTF8;
            byte[] buf = enc.GetBytes(arg[0].ConvertString());
            m_Response.OutputStream.Write(buf, 0, buf.Length);
            return new BSObject(null);
        }

        private ABSObject ResponseAddHeader(ABSObject[] arg)
        {
            m_Response.AddHeader(arg[0].ConvertString(), arg[1].ConvertString());
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
    }
}
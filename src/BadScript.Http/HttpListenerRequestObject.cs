using System.Collections.Generic;
using System.IO;
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
    public class HttpListenerRequestObject : ABSObject
    {
        private BSTable m_InstanceFunctions;
        private HttpListenerRequest m_Request;

        public override bool IsNull => false;

        #region Public

        public HttpListenerRequestObject( SourcePosition pos, HttpListenerRequest request) : base( pos )
        {
            m_Request = request;


            Dictionary<ABSObject, ABSObject> headers = new Dictionary<ABSObject, ABSObject>();
            foreach (string header in m_Request.Headers)
            {
                headers[new BSObject(header)] =
                    new BSArray(m_Request.Headers.GetValues(header).Select(x => new BSObject(x)));
            }

            Dictionary<ABSObject, ABSObject> queryTable = new Dictionary<ABSObject, ABSObject>();
            foreach (string s in m_Request.QueryString)
            {
                string[] vals = m_Request.QueryString.GetValues(s);
                if(vals!=null)
                queryTable[new BSObject(s)] = new BSArray(vals.Select(x => new BSObject(x)));
            }

            m_InstanceFunctions = new BSTable(
                SourcePosition.Unknown,
                new Dictionary < ABSObject, ABSObject >
                {
                    {new BSObject("headers"), new BSTable(SourcePosition.Unknown, headers)},
                    {new BSObject("uri"), new BSObject(m_Request.Url.ToString())},
                    {new BSObject("acceptTypes"), new BSArray((m_Request.AcceptTypes ?? new string[0]).Select(x=>new BSObject(x)))},
                    {new BSObject("contentLength"), new BSObject((decimal)m_Request.ContentLength64)},
                    {new BSObject("contentType"), new BSObject(m_Request.ContentType)},
                    {new BSObject("httpMethod"), new BSObject(m_Request.HttpMethod)},
                    {new BSObject("query"), new BSTable(SourcePosition.Unknown, queryTable)},
                    {new BSObject("readBody"), new BSFunction("function readBody()", RequestReadBody,0 ) },
                }
            );
        }

        private ABSObject RequestReadBody(ABSObject[] arg)
        {
            Encoding enc = m_Request.ContentEncoding ?? Encoding.UTF8;
            using (BinaryReader r = new BinaryReader(m_Request.InputStream))
                return new BSObject(enc.GetString(r.ReadBytes((int)m_Request.ContentLength64)));
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
    }
}
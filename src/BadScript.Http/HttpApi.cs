using System.IO;
using System.Net;
using System.Text;

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Http
{

    public static class HttpApi
    {

        #region Public

        public static void AddApi()
        {
            BSTable t = new BSTable();
            t.InsertElement( new BSObject( "get" ), new BSFunction( "function get(url)", Get ) );

            t.InsertElement(
                            new BSObject( "post" ),
                            new BSFunction( "function post(url, body)", Post )
                           );

            t.InsertElement(
                            new BSObject( "downloadFile" ),
                            new BSFunction( "function downloadFile(url, destination)", DownloadFile )
                           );

            t.InsertElement(
                            new BSObject( "downloadString" ),
                            new BSFunction( "function downloadString(url)", DownloadString )
                           );

            BSEngine.AddStatic( "http", t );
        }

        #endregion

        #region Private

        private static ABSObject DownloadFile( ABSObject[] args )
        {
            string url = args[0].ConvertString();
            string file = args[1].ConvertString();

            using ( WebClient wc = new WebClient() )
            {
                wc.DownloadFile( url, file );
            }

            return new BSObject( null );
        }

        private static ABSObject DownloadString( ABSObject[] args )
        {
            string url = args[0].ConvertString();

            using ( WebClient wc = new WebClient() )
            {
                wc.DownloadString( url );
            }

            return new BSObject( null );
        }

        private static ABSObject Get( ABSObject[] args )
        {
            string url = args[0].ConvertString();
            HttpWebRequest request = WebRequest.CreateHttp( url );
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = ( HttpWebResponse ) request.GetResponse();
            BSTable t = new BSTable();

            t.InsertElement(
                            new BSObject( "status" ),
                            new BSObject( ( decimal ) response.StatusCode )
                           );

            using ( TextReader tr = new StreamReader( response.GetResponseStream() ) )
            {
                t.InsertElement( new BSObject( "body" ), new BSObject( tr.ReadToEnd() ) );
            }

            return t;
        }

        private static ABSObject Post( ABSObject[] args )
        {
            string url = args[0].ConvertString();
            HttpWebRequest request = WebRequest.CreateHttp( url );
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;

            string dataStr = args[1].ConvertString();
            byte[] data = Encoding.UTF8.GetBytes( dataStr );

            using ( Stream stream = request.GetRequestStream() )
            {
                stream.Write( data, 0, data.Length );
            }

            HttpWebResponse response = ( HttpWebResponse ) request.GetResponse();
            BSTable t = new BSTable();

            t.InsertElement(
                            new BSObject( "status" ),
                            new BSObject( ( decimal ) response.StatusCode )
                           );

            using ( TextReader tr = new StreamReader( response.GetResponseStream() ) )
            {
                t.InsertElement( new BSObject( "body" ), new BSObject( tr.ReadToEnd() ) );
            }

            return t;
        }

        #endregion

    }

}

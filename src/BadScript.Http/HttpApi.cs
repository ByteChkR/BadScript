using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Http
{
    public static class HttpApi
    {

        public static void AddApi()
        {
            EngineRuntimeTable t = new EngineRuntimeTable();
            t.InsertElement(new EngineRuntimeObject("get"), new BSRuntimeFunction("function get(url)", Get));
            t.InsertElement(new EngineRuntimeObject("post"), new BSRuntimeFunction("function post(url, body)", Post));
            t.InsertElement(new EngineRuntimeObject("downloadFile"), new BSRuntimeFunction("function downloadFile(url, destination)", DownloadFile));
            t.InsertElement(new EngineRuntimeObject("downloadString"), new BSRuntimeFunction("function downloadString(url)", DownloadString));
            BSEngine.AddStatic( "http", t );
        }


        private static BSRuntimeObject Post(BSRuntimeObject[] args)
        {
            string url = args[0].ConvertString();
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "POST";
            request.Credentials = CredentialCache.DefaultCredentials;

            string dataStr = args[1].ConvertString();
            byte[] data = Encoding.UTF8.GetBytes( dataStr );
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            EngineRuntimeTable t = new EngineRuntimeTable();

            t.InsertElement(
                            new EngineRuntimeObject("status"),
                            new EngineRuntimeObject((decimal)response.StatusCode)
                           );

            using (TextReader tr = new StreamReader(response.GetResponseStream()))
                t.InsertElement(new EngineRuntimeObject("body"), new EngineRuntimeObject(tr.ReadToEnd()));

            return t;
        }


        private static BSRuntimeObject Get(BSRuntimeObject[] args)
        {
            string url = args[0].ConvertString();
            HttpWebRequest request = WebRequest.CreateHttp( url );
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            EngineRuntimeTable t = new EngineRuntimeTable();

            t.InsertElement(
                            new EngineRuntimeObject( "status" ),
                            new EngineRuntimeObject((decimal)response.StatusCode)
                           );

            using ( TextReader tr = new StreamReader( response.GetResponseStream() ) )
                t.InsertElement( new EngineRuntimeObject( "body" ), new EngineRuntimeObject( tr.ReadToEnd() ) );

            return t;
        }

        private static BSRuntimeObject DownloadFile(BSRuntimeObject[] args)
        {
            string url = args[0].ConvertString();
            string file = args[1].ConvertString();

            using (WebClient wc = new WebClient())
                wc.DownloadFile(url, file);

            return new EngineRuntimeObject(null);
        }
        private static BSRuntimeObject DownloadString(BSRuntimeObject[] args)
        {
            string url = args[0].ConvertString();
            using (WebClient wc = new WebClient())
                wc.DownloadString(url);

            return new EngineRuntimeObject(null);
        }
    }
}

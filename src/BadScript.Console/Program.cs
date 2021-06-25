using System;
using System.Linq;
using BadScript.Http;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.Zip;

namespace BadScript.Console
{

    internal class Program
    {
        #region Private

        private static void Main( string[] args )
        {
            BSJsonApi.AddApi( BSJsonApiSettings.Full );
            ZipApi.AddApi();
            BSFileSystemApi.AddApi();
            BSMathApi.AddApi();
            ProcessApi.AddApi();
            HttpApi.AddApi();
            BSEngineInstance engine = BSEngine.CreateEngineInstance();

            string a = "";

            foreach ( string s in args )
            {
                a += " " + s;
            }

            string[] ar = a.Split( ';' );

            foreach ( string s in ar )
            {
                string[] parts = s.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
                engine.LoadFile( parts[0], parts.Skip( 1 ).ToArray() );
            }
        }

        #endregion
    }

}

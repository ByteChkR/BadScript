using System;
using System.Linq;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Zip;

namespace BadScript.Console
{

    internal class Program
    {
        #region Private

        private static string LoadConfig(BSEngineInstance i, string src)
        {
            return i.LoadFile( src, new string[0] ).ConvertString();
        }

        private static void Main( string[] args )
        {
            BadScriptCoreApi.AddApi();
            ConsoleApi.AddApi();
            BSJsonApi.AddApi( BSJsonApiSettings.Full );
            ZipApi.AddApi();
            BSFileSystemApi.AddApi();
            BSMathApi.AddApi();
            ProcessApi.AddApi();
            HttpApi.AddApi();
            StringUtilsApi.AddApi();
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

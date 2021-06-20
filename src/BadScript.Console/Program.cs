using System;
using System.Collections.Generic;
using System.Linq;

using BadScript.Apis.FileSystem;
using BadScript.Apis.Math;
using BadScript.Http;
using BadScript.Json;
using BadScript.Process;
using BadScript.Runtime;
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
                a += " "+s;
            }

            string[] ar = a.Split( ';' );

            foreach ( string s in ar)
            {
                string[] parts = s.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
                engine.LoadFile( parts[0], parts.Skip(1).ToArray());
            }

            System.Console.WriteLine( "Execution Finished." );
            System.Console.WriteLine( "Global Table: " );
            System.Console.WriteLine( engine.GlobalTable.SafeToString() );
        }

        #endregion

    }

}

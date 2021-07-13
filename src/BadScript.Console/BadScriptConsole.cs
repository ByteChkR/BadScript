using System;
using System.IO;
using System.Linq;
using BadScript.Console.IO;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.Imaging;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Zip;

namespace BadScript.Console
{

    internal class BadScriptConsole
    {
        private static readonly IConsoleIORoot m_IORoot =
            new ConsoleIORoot( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "configs/" ) );

        #region Private

        private static string GetConsoleIncludeDir()
        {
            string p = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "include" );
            Directory.CreateDirectory( p );

            return p;
        }

        private static string[] GetDefaultInterfaces()
        {
            IConsoleIOFile configFile = new ConsoleIOFile( "interfaces.json", m_IORoot, null );
            configFile.EnsureExistParent();

            if ( configFile.Exists )
            {
                string[] ret = configFile.ParseJson < string[] >();

                return ret;
            }
            else
            {
                string[] ret = new[] { "#core", "#console" };
                configFile.WriteJson( ret );

                return ret;
            }
        }

        private static string LoadConfig( BSEngineInstance i, string src )
        {
            return i.LoadFile( src, new string[0] ).ConvertString();
        }

        private static void Main( string[] args )
        {
            BSEngine.AddStatic( new BS2JsonInterface() );
            BSEngine.AddStatic( new BSFileSystemInterface() );
            BSEngine.AddStatic( new BSFileSystemPathInterface() );
            BSEngine.AddStatic( new BadScriptCoreApi() );
            BSEngine.AddStatic( new BSMathApi() );
            BSEngine.AddStatic( new ConsoleApi() );
            BSEngine.AddStatic( new HttpApi() );
            BSEngine.AddStatic( new Json2BSInterface() );
            BSEngine.AddStatic( new ProcessApi() );
            BSEngine.AddStatic( new StringUtilsApi() );
            BSEngine.AddStatic( new ZipApi() );
            BSEngine.AddStatic( new ImagingApi() );
            BSEngineInstance engine = BSEngine.CreateEngineInstance( GetDefaultInterfaces(), GetConsoleIncludeDir() );

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

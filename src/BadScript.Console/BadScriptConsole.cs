using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Console.IO;
using BadScript.Console.Plugins;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Utils;
using BadScript.Zip;

namespace BadScript.Console
{

    internal class BadScriptConsole
    {
        private static readonly ConsoleIORoot m_IORoot =
            new( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "bs-data/" ) );
        public static readonly ConsoleIODirectory AppDirectory = new( "apps", m_IORoot, null );
        public static readonly ConsoleIODirectory IncludeDirectory = new( "include", m_IORoot, null );
        private static PluginLoader m_PluginLoader;

        #region Private

        private static string GetConsoleIncludeDir()
        {
            IncludeDirectory.EnsureExistsSelf();

            return IncludeDirectory.GetFullName();
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
                string[] ret = { "#core", "#console" };
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


            m_PluginLoader = new PluginLoader( m_IORoot, new ConsoleIODirectory( "plugins", m_IORoot, null ) );

            m_PluginLoader.LoadPlugins();
            BSEngine.AddStatic( new BS2JsonInterface() );
            BSEngine.AddStatic( new BSFileSystemInterface() );
            BSEngine.AddStatic( new BSFileSystemPathInterface( AppDomain.CurrentDomain.BaseDirectory ) );
            BSEngine.AddStatic( new BadScriptCoreApi() );
            BSEngine.AddStatic( new BSMathApi() );
            BSEngine.AddStatic( new ConsoleApi() );
            BSEngine.AddStatic( new ConsoleColorApi() );
            BSEngine.AddStatic( new HttpApi() );
            BSEngine.AddStatic( new HttpServerApi() );
            BSEngine.AddStatic( new Json2BSInterface() );
            BSEngine.AddStatic( new ProcessApi() );
            BSEngine.AddStatic( new StringUtilsApi() );
            BSEngine.AddStatic( new ZipApi() );
            BSEngine.AddStatic( new ImagingApi() );
            BSEngine.AddStatic( new BSReflectionScriptInterface() );



            AppDirectory.EnsureExistsSelf();

            BSEngineInstance engine = BSEngine.CreateEngineInstance( GetDefaultInterfaces(), GetConsoleIncludeDir() );

            string a = "";

            foreach ( string s in args )
            {
                a += " " + s;
            }

            string[] ar = a.Split( new char[]{ ';' , ','} );

            List < ConsoleExecution > executions = new List < ConsoleExecution >();

            foreach ( string execStr in ar )
            {
                string[] parts = execStr.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
                executions.Add( new ConsoleExecution( parts[0], parts.Skip( 1 ).ToArray() ) );
            }


            foreach ( ConsoleExecution consoleExecution in executions )
            {
                consoleExecution.Run( engine );
            }
            
        }

        #endregion
    }

}

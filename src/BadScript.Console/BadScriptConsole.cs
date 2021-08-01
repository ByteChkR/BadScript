using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
using BadScript.Utils.Reflection;
using BadScript.Zip;

namespace BadScript.Console
{

    internal class BadScriptConsole
    {
        private static readonly Dictionary < string, Assembly > s_VersionTags = new Dictionary < string, Assembly >
        {
            { "CLI", typeof( BadScriptConsole ).Assembly },
            { "Runtime", typeof( BSEngine ).Assembly },
            { "BadScript.ConsoleUtils", typeof( ConsoleApi ).Assembly },
            { "BadScript.Core", typeof( BadScriptCoreApi ).Assembly },
            { "BadScript.Http", typeof( HttpApi ).Assembly },
            { "BadScript.HttpServer", typeof( HttpServerApi ).Assembly },
            { "BadScript.Imaging", typeof( ImagingApi ).Assembly },
            { "BadScript.IO", typeof( BSFileSystemInterface ).Assembly },
            { "BadScript.Json", typeof( Json2BSInterface ).Assembly },
            { "BadScript.Math", typeof( BSMathApi ).Assembly },
            { "BadScript.Process", typeof( ProcessApi ).Assembly },
            { "BadScript.StringUtils", typeof( StringUtilsApi ).Assembly },
            { "BadScript.Zip", typeof( ZipApi ).Assembly },
            { "BadScript.Utils", typeof( BSVersionObject ).Assembly },
        };

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

        private static void PrintSyntax()
        {
            System.Console.WriteLine("Syntax: bs <app/filepath>");
            System.Console.WriteLine("Syntax: bs --help");
            System.Console.WriteLine("Syntax: bs --version");
        }
        
        private static void Initialize()
        {
            m_PluginLoader = new PluginLoader(m_IORoot, new ConsoleIODirectory("plugins", m_IORoot, null));

            m_PluginLoader.LoadPlugins();
            BSEngine.AddStatic(new BS2JsonInterface());
            BSEngine.AddStatic(new BSFileSystemInterface());
            BSEngine.AddStatic(new BSFileSystemPathInterface(AppDomain.CurrentDomain.BaseDirectory));
            BSEngine.AddStatic(new BadScriptCoreApi());
            BSEngine.AddStatic(new BSMathApi());
            BSEngine.AddStatic(new ConsoleApi());
            BSEngine.AddStatic(new ConsoleColorApi());
            BSEngine.AddStatic(new HttpApi());
            BSEngine.AddStatic(new HttpServerApi());
            BSEngine.AddStatic(new Json2BSInterface());
            BSEngine.AddStatic(new ProcessApi());
            BSEngine.AddStatic(new StringUtilsApi());
            BSEngine.AddStatic(new ZipApi());
            BSEngine.AddStatic(new ImagingApi());
            BSEngine.AddStatic(new BSReflectionScriptInterface());
            BSEngine.AddStatic(new VersionToolsInterface());

            AppDirectory.EnsureExistsSelf();

        }

        private static void PrintHeaderInfo()
        {
            string h =
                $"Bad Script Console (CLI: {typeof( BadScriptConsole ).Assembly.GetName().Version}, Runtime: {typeof( BSEngine ).Assembly.GetName().Version})\n";

            System.Console.WriteLine( h );
        }

        private static void PrintVersionInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "Versions:" );

            foreach ( KeyValuePair < string, Assembly > keyValuePair in s_VersionTags )
            {
                sb.AppendLine( "\t" + keyValuePair.Key + ": " + keyValuePair.Value.GetName().Version );
            }

            System.Console.WriteLine( sb.ToString() );
        }

        private static void Main( string[] args )
        {
            PrintHeaderInfo();
            Initialize();

            if ( args.Length == 0 )
            {
                System.Console.WriteLine( "No Arguments." );
                PrintSyntax();

                return;
            }
            else if ( args[0] == "--help" )
            {
                PrintSyntax();

                return;
            }
            else if ( args[0] == "--version" )
            {
                PrintVersionInfo();

                return;
            }


            BSEngineInstance engine = BSEngine.CreateEngineInstance( GetDefaultInterfaces(), GetConsoleIncludeDir() );

            string a = "";

            foreach ( string s in args )
            {
                a += " " + s;
            }

            string[] ar = a.Split( new char[] { ';', ',' } );

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

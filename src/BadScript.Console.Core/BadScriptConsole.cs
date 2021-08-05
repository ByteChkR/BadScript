using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BadScript.Console.Core.Settings;
using BadScript.Interfaces;

namespace BadScript.Console.Core
{

    public class BadScriptConsole
    {
        public readonly BadScriptConsoleSettings Settings;

        #region Public

        public BadScriptConsole( BadScriptConsoleSettings settings )
        {
            Settings = settings;
            PrintHeaderInfo();
        }

        public void Run( string[] args )
        {
            bool isBenchmark = false;

            if ( args.Length == 0 )
            {
                Settings.ConsoleOutput.WriteLine( "No Arguments." );
                PrintSyntax();

                return;
            }
            else if ( args[0] == "--help" ||
                      args[0] == "-h" )
            {
                PrintSyntax();

                return;
            }
            else if ( args[0] == "--version" ||
                      args[0] == "-v" )
            {
                PrintVersionInfo();

                return;
            }
            else if ( args[0] == "--benchmark" ||
                      args[0] == "-b" )
            {
                args = args.Skip( 1 ).ToArray();
                isBenchmark = true;
            }

            BSEngine engine = Settings.EngineSettings.Build();

            if ( Settings.AddEnvironmentApi )
            {
                engine.AddInterface( new BSEngineEnvironmentInterface( engine ) );
            }

            string a = "";

            foreach ( string s in args )
            {
                a += " " + s;
            }

            string[] ar = a.Split( new char[] { ';', ',' } );

            List < ConsoleExecution > executions = new List < ConsoleExecution >();

            foreach ( string execStr in ar )
            {
                string[] parts = execStr.Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

                if ( Directory.Exists( parts[0] ) )
                {
                    string[] files = Directory.GetFiles( parts[0], "*.bs", SearchOption.AllDirectories );

                    executions.AddRange(
                        files.Select( x => new ConsoleExecution( this, x, parts.Skip( 1 ).ToArray(), isBenchmark ) ) );
                }
                else
                {
                    executions.Add( new ConsoleExecution( this, parts[0], parts.Skip( 1 ).ToArray(), isBenchmark ) );
                }
            }

            foreach ( ConsoleExecution consoleExecution in executions )
            {
                consoleExecution.Run( engine );
            }

        }

        #endregion

        #region Private

        private void PrintHeaderInfo()
        {
            string h =
                $"Bad Script Console (CLI: {typeof( BadScriptConsole ).Assembly.GetName().Version}, Runtime: {typeof( BSEngine ).Assembly.GetName().Version})\n\nLoading Runtime...\n";

            Settings.ConsoleOutput.WriteLine( h );
        }

        private void PrintSyntax()
        {
            Settings.ConsoleOutput.WriteLine( "Syntax: bs <app/filepath>" );
            Settings.ConsoleOutput.WriteLine( "Syntax: bs --help" );
            Settings.ConsoleOutput.WriteLine( "Syntax: bs --version" );
            Settings.ConsoleOutput.WriteLine( "Syntax: bs --benchmark <app/filepath>" );
        }

        private void PrintVersionInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "Versions:" );

            foreach ( ( string, Version ) keyValuePair in Settings.GetVersions() )
            {
                sb.AppendLine( "\t" + keyValuePair.Item1 + ": " + keyValuePair.Item2 );
            }

            Settings.ConsoleOutput.WriteLine( sb.ToString() );
        }

        #endregion
    }

}

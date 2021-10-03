using System;
using System.Linq;

using BadScript.ConsoleUtils;

namespace BadScript.Examples.SimpleConsole
{

    internal class SimpleConsoleExample
    {

        #region Private

        private static BSEngine CreateEngine()
        {
            Console.WriteLine( "Creating Script Engine" );
            BSEngineSettings settings = BSEngineSettings.MakeDefault();

            settings.Interfaces.Add( new BSConsoleInterface() ); //Add the Console API so we can write things

            return settings.Build();
        }

        private static void Main( string[] args )
        {
            BSEngine engine = CreateEngine();

            if ( args.Length == 0 )
            {
                Console.WriteLine( "No File Specified" );

                return;
            }

            string script = args[0];
            string[] scriptArgs = args.Skip( 1 ).ToArray(); //Skip Script Name for convenience

            engine.LoadFile( script, scriptArgs );
        }

        #endregion

    }

}

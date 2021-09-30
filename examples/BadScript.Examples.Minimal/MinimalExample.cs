using System;

using BadScript.Common.Types;
using BadScript.Settings;

namespace BadScript.Examples.Minimal
{

    internal class MinimalExample
    {

        #region Private

        private static BSEngine CreateEngine()
        {
            Console.WriteLine( "Creating Script Engine" );
            BSEngineSettings settings = BSEngineSettings.MakeDefault();

            return settings.Build();
        }

        private static void Main( string[] args )
        {
            BSEngine engine = CreateEngine();

            string source = "return 92253/1337";

            Console.WriteLine( $"Source: {source}" );

            ABSObject returnValue = engine.LoadSource( source );

            Console.WriteLine( $"Output: {returnValue}" );
        }

        #endregion

    }

}

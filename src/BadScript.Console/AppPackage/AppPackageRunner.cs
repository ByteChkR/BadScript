using System;
using System.Linq;

using BadScript.Console.Logging;
using BadScript.Console.Subsystems;
using BadScript.Types;

namespace BadScript.Console.AppPackage
{

    public static class AppPackageRunner
    {

        #region Public

        public static int RunPackage( AppPackageRunnerSettings settings )
        {
            BSAppPackage package = new BSAppPackage( settings.AppPath );

            BSEngineSettings es =
                EngineBuilderSettings.CreateEngineSettings(
                                                           false,
                                                           Array.Empty < string >(),
                                                           package.Manifest.RequiredInterfaces
                                                          );

            es.Interfaces.Add( new BSAppPackageInterface( package ) );
            es.ActiveInterfaces.Add( "app" );

            BSEngine engine = es.Build();
            ABSObject o;

            try
            {
                o = engine.LoadSource( package.GetSource(), settings.Arguments.ToArray() );
            }
            catch ( Exception e )
            {
                ConsoleWriter.ErrorLine( "App Crashed: " + e.Message );

                return -1;
            }

            ConsoleWriter.SuccessLine( $"App Terminated with Return: {o}" );

            return 0;
        }

        #endregion

    }

}

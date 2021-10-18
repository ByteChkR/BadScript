using System;
using System.IO;
using System.Linq;

using BadScript.Console.Logging;
using BadScript.Console.Subsystems;
using BadScript.Parser.Expressions;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

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
                                                           Array.Empty < string >(),
                                                           Array.Empty < string >()
                                                          );

            es.Interfaces.Add( new BSAppPackageInterface( package ) );
            es.ActiveInterfaces.Add( "App" );
            es.ActiveInterfaces.AddRange( package.Manifest.RequiredInterfaces );

            ABSObject o;

            try
            {
                if ( settings.Secure )
                {
                    BSScope scope = es.BuildLocalEnvironment();
                    BSEngine engine = scope.Engine;

                    //Manually Load APP interface because local environment will not load the active interfaces
                    scope.AddLocalVar(
                                      "App",
                                      engine.LoadInterface( "App", new BSTable( SourcePosition.Unknown ) )
                                     );

                    o = engine.LoadSource( package.GetSource(), scope, settings.Arguments.ToArray() );
                }
                else
                {
                    BSEngine engine = es.Build();
                    o = engine.LoadSource( package.GetSource(), settings.Arguments.ToArray() );
                }
            }
            catch ( Exception e )
            {
                ConsoleWriter.ErrorLine( "App Crashed: " + e.Message );

                return -1;
            }

            if ( Directory.Exists( package.Manifest.GetTempDirectory() ) )
            {
                Directory.Delete( package.Manifest.GetTempDirectory() );
            }

            if ( !settings.NoLogo )
            {
                ConsoleWriter.SuccessLine( $"App Terminated with Return: {o}" );
            }

            return 0;
        }

        #endregion

    }

}

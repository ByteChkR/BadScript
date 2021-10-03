﻿using System.Collections.Generic;
using System.Linq;

using BadScript.Console.AppPackage;
using BadScript.Console.Logging;
using BadScript.Console.Subsystems.Compile;
using BadScript.Console.Subsystems.Include;
using BadScript.Console.Subsystems.Project;
using BadScript.Console.Subsystems.Run;

using CommandLine;

namespace BadScript.Console
{

    public class BSConsole
    {

        #region Private

        private static int HandleError( IEnumerable < Error > errs )
        {
            return -1;
        }

        private static int Main( string[] args )
        {
            if ( args.Length > 0 && args[0] == "project" )
            {
                return CommandLine.Parser.Default.
                                   ParseArguments < ProjectCreatorSettings, ProjectBuilderSettings >(
                                        args.Skip( 1 ).ToArray()
                                       ).
                                   MapResult(
                                             ( ProjectCreatorSettings o ) =>
                                             {
                                                 if ( !o.NoLogo )
                                                 {
                                                     PrintHeaderInfo();
                                                 }

                                                 return ProjectCreator.Create( o );
                                             },
                                             ( ProjectBuilderSettings o ) =>
                                             {
                                                 if ( !o.NoLogo )
                                                 {
                                                     PrintHeaderInfo();
                                                 }

                                                 return ProjectBuilder.Build( o );
                                             },
                                             HandleError
                                            );
            }

            return CommandLine.Parser.Default.
                               ParseArguments < ScriptRunnerSettings, ScriptCompilerSettings,
                                   IncludeManagerSettings, AppPackageRunnerSettings, AppBuilderSettings >( args ).
                               MapResult(
                                         ( ScriptRunnerSettings o ) =>
                                         {
                                             if ( !o.NoLogo )
                                             {
                                                 PrintHeaderInfo();
                                             }

                                             return ScriptRunner.Run( o );
                                         },
                                         ( ScriptCompilerSettings o ) =>
                                         {
                                             if ( !o.NoLogo )
                                             {
                                                 PrintHeaderInfo();
                                             }

                                             return ScriptCompiler.Compile( o );
                                         },
                                         (IncludeManagerSettings o) =>
                                         {
                                             if (!o.NoLogo)
                                             {
                                                 PrintHeaderInfo();
                                             }

                                             return IncludeManager.Process(o);
                                         },
                                         (AppPackageRunnerSettings o) =>
                                         {
                                             if (!o.NoLogo)
                                             {
                                                 PrintHeaderInfo();
                                             }

                                             return AppPackageRunner.RunPackage(o);
                                         },
                                         (AppBuilderSettings o) =>
                                         {
                                             if (!o.NoLogo)
                                             {
                                                 PrintHeaderInfo();
                                             }

                                             return AppBuilder.Build(o);
                                         },
                                         HandleError
                                        );
        }

        private static void PrintHeaderInfo()
        {
            string h =
                $"Bad Script Console (CLI: {typeof( BSConsoleDirectories ).Assembly.GetName().Version}, Runtime: {typeof( BSEngine ).Assembly.GetName().Version})\n\nLoading Runtime...\n";

            ConsoleWriter.SuccessLine( h );
        }

        #endregion

    }

}

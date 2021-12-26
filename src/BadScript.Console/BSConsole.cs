using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Console.AppPackage;
using BadScript.Console.Logging;
using BadScript.Plugins;
using BadScript.Console.Subsystems.Compile;
using BadScript.Console.Subsystems.Include;
using BadScript.Console.Subsystems.Project;
using BadScript.Console.Subsystems.Run;
using BadScript.Interfaces.Environment.Settings;
using BadScript.Settings;
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

        private static void LoadPlugins()
        {
            SettingsCategory cat = BSSettings.BsRoot.FindCategory("core.settings.plugins", true);
            bool enable = cat.GetSetting("enable", "true").Value == "true";
            if (!enable) return;

            if (!cat.HasSetting("path"))
            {
                cat.SetSetting("path", Path.Combine(BSConsoleDirectories.Instance.DataDirectory, "plugins"));
            }

            SettingsPair pair = cat.GetSetting("path");

            Directory.CreateDirectory(pair.Value);
            PluginManager.InitializePlugins(pair.Value);
        }
        
        private static int Main( string[] args )
        {
            PluginManager.OnLog += ConsoleWriter.LogLine;
            BSSettings.BsRoot.LoadFromDirectory( BSConsoleDirectories.Instance.SettingsDirectory );

            LoadPlugins();
            

            int ret = 0;

            if ( args.Length > 0 && args[0] == "project" )
            {
                ret = CommandLine.Parser.Default.
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
            else
            {
                ret = CommandLine.Parser.Default.
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
                                            ( IncludeManagerSettings o ) =>
                                            {
                                                if ( !o.NoLogo )
                                                {
                                                    PrintHeaderInfo();
                                                }

                                                return IncludeManager.Process( o );
                                            },
                                            ( AppPackageRunnerSettings o ) =>
                                            {
                                                if ( !o.NoLogo )
                                                {
                                                    PrintHeaderInfo();
                                                }

                                                return AppPackageRunner.RunPackage( o );
                                            },
                                            ( AppBuilderSettings o ) =>
                                            {
                                                if ( !o.NoLogo )
                                                {
                                                    PrintHeaderInfo();
                                                }

                                                return AppBuilder.Build( o );
                                            },
                                            HandleError
                                           );
            }

            BSSettings.BsRoot.SaveToDirectory( BSConsoleDirectories.Instance.SettingsDirectory );

            return ret;
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

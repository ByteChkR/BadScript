﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BadScript.Console.Logging;
using BadScript.Console.Preprocessor;
using BadScript.Console.Subsystems.Project.BuildFormats;
using BadScript.Console.Subsystems.Project.DataObjects;
using BadScript.Console.Subsystems.Project.Utils;
using BadScript.Exceptions;
using BadScript.Parser;
using BadScript.Parser.Expressions;

namespace BadScript.Console.Subsystems.Project
{

    public static class ProjectBuilder
    {

        #region Public

        public static int Build( ProjectBuilderSettings settings )
        {
            SourcePreprocessorDirectories.InitializeIfNull();
            string p = "./build-settings.json";
            ProjectSettings ps = ProjectSettings.Deserialize( File.ReadAllText( p ) );
            ps.SaveLocation = p;
            BuildTarget t = ps.BuildTargets.GetTarget( settings.BuildTarget );

            Build( Path.GetDirectoryName( Path.GetFullPath( p ) ), ps, t );

            ConsoleWriter.SuccessLine( "Command 'project make' finished!" );

            return 0;
        }

        #endregion

        #region Private

        private static string Build( string workingDir, ProjectSettings s, BuildTarget t )
        {
            ConsoleWriter.LogLine(
                                  $"Building Project: {s.ResolveValue( "%AppInfo.Name%@%Target.Name%", t.Name )}"
                                 );

            string oldDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory( workingDir );
            ConsoleWriter.LogLine( "\tRunning Pre-Events.." );

            foreach ( string tPreEvent in t.PreEvents )
            {
                string cmd = s.ResolveValue( tPreEvent, t.Name );

                ProcessEventRunner pev = new ProcessEventRunner(
                                                                "PRE_EVENT",
                                                                cmd
                                                               );

                if ( pev.Run() == ProcessEventRunner.ProcessExitMode.Failed )
                {
                    ConsoleWriter.ErrorLine( "\tPre Event Failed! Aborting.." );

                    return null;
                }
            }

            //Reload Project Settings
            s.Update();

            //Reload Target
            t = s.BuildTargets.GetTarget( t.Name );

            if ( !string.IsNullOrEmpty( t.SubTarget ) )
            {
                Build( workingDir, s, s.BuildTargets.GetTarget( t.SubTarget ) );
            }

            List < string > referencedFiles = new List < string >();
            string[] refResolvers = s.ReferenceResolvers.Keys.ToArray();

            ConsoleWriter.LogLine( "\tProcessing References.." );

            foreach ( BuildTargetReference buildTargetReference in t.References )
            {
                void ResolveReferenceProperty( PropertyResolveEventArgs obj )
                {
                    if ( obj.Input == "Reference" )
                    {
                        obj.Cancel = true;
                        obj.Result = buildTargetReference.ResolveProperty( obj.Current + 1, obj.Parts, obj.Info );
                    }
                }

                string resolverType = refResolvers.FirstOrDefault( x => buildTargetReference.Path.StartsWith( x ) );

                if ( resolverType != null )
                {
                    s.OnResolveProperty += ResolveReferenceProperty;

                    ProcessEventRunner evr = new ProcessEventRunner(
                                                                    "REF_RESOLVER",
                                                                    s.ResolveValue(
                                                                         s.ReferenceResolvers[resolverType],
                                                                         t.Name
                                                                        ),
                                                                    true,
                                                                    2
                                                                   );

                    evr.Run();
                    referencedFiles.Add( evr.OutputData );

                    s.OnResolveProperty -= ResolveReferenceProperty;
                }
                else
                {
                    if ( buildTargetReference.Target == "none" )
                    {
                        continue;
                    }

                    string wDir = Path.GetDirectoryName( Path.GetFullPath( buildTargetReference.Path ) );
                    ProjectSettings subSettings = buildTargetReference.GetSettings( s, t.Name );
                    subSettings.SaveLocation = Path.GetFullPath( buildTargetReference.Path );
                    string target = s.ResolveValue( buildTargetReference.Target, t.Name );
                    BuildTarget targ = subSettings.BuildTargets.GetTarget( target );

                    string file = Build(
                                        wDir,
                                        subSettings,
                                        targ
                                       );

                    file = subSettings.ResolveValue( targ.OutputFile, targ.Name );

                    if ( file != null )
                    {
                        referencedFiles.Add(
                                            "./" +
                                            Path.GetRelativePath(
                                                                 workingDir,
                                                                 Path.GetFullPath( Path.Combine( wDir, file ) )
                                                                ).
                                                 Replace( "\\", "/" )
                                           );
                    }
                }
            }

            string outFile = "";

            if ( t.OutputFormat != "none" )
            {
                ConsoleWriter.LogLine( "\tFinding Included Files..." );
                string source = GenerateIncludeFiles( s, t, referencedFiles );

                ConsoleWriter.LogLine( "\tRunning Preprocessor..." );

                string dirs = "";

                if ( s.PreprocessorDirectives != null )
                {
                    dirs += s.ResolveValue( s.PreprocessorDirectives, t.Name );
                }

                if ( t.PreprocessorDirectives != null )
                {
                    dirs += " " + s.ResolveValue( t.PreprocessorDirectives, t.Name );
                }

                string src = SourcePreprocessor.Preprocess(
                                                           source,
                                                           dirs
                                                          );

                ConsoleWriter.LogLine( "\tProcessing Source..." );
                outFile = ProcessGeneratedFile( s, t, src );
            }
            else
            {
                ConsoleWriter.WarnLine( "\tNo Build Format. Skipping Build." );
            }

            ConsoleWriter.LogLine( "\tRunning Post-Events.." );

            foreach ( string tPreEvent in t.PostEvents )
            {
                string cmd = s.ResolveValue( tPreEvent, t.Name );

                ProcessEventRunner pev = new ProcessEventRunner(
                                                                "POST_EVENT",
                                                                cmd
                                                               );

                if ( pev.Run() == ProcessEventRunner.ProcessExitMode.Failed )
                {
                    ConsoleWriter.ErrorLine( "\tPost Event Failed!" );
                }
            }

            if ( t.TestOutput )
            {
                try
                {
                    if ( !string.IsNullOrWhiteSpace( outFile ) && outFile.EndsWith( ".bs" ) && File.Exists( outFile ) )
                    {
                        BSParser parser = new BSParser( File.ReadAllText( outFile ) );
                        BSExpression[] exprs = parser.ParseToEnd();

                        ConsoleWriter.SuccessLine(
                                                  $"Parsing Test Completed: {s.ResolveValue( "%Target.OutputFile%", t.Name )} ({exprs.Length} Expressions)"
                                                 );
                    }
                    else
                    {
                        ConsoleWriter.WarnLine(
                                               $"Parsing Test Failed: No Output was Generated for Project {s.ResolveValue( "%Target.OutputFile%", t.Name )}"
                                              );
                    }
                }
                catch ( BSParserException e )
                {
                    ConsoleWriter.ErrorLine(
                                            $"Parsing Test Failed: {s.ResolveValue( "%Target.OutputFile%", t.Name )}\n{e}"
                                           );

                    throw;
                }
            }

            Directory.SetCurrentDirectory( oldDir );

            ConsoleWriter.SuccessLine(
                                      $"Finished Building Project: {s.ResolveValue( "%AppInfo.Name%@%Target.Name%", t.Name )}"
                                     );

            return outFile;
        }

        private static string GenerateIncludeFiles( ProjectSettings settings, BuildTarget t, List < string > refs )
        {
            StringBuilder sb = new StringBuilder();

            string log = "\tIncluding Files: \n\t\t";

            List < string > includeFiles = new List < string >();

            foreach ( string s in t.Include )
            {
                string inc = settings.ResolveValue( s, t.Name );

                if ( inc.Contains( '*' ) )
                {
                    int idx = inc.IndexOf( '*' );

                    includeFiles.AddRange(
                                          Directory.GetFiles(
                                                             inc.Substring( 0, idx ),
                                                             inc.Substring(
                                                                           idx,
                                                                           inc.Length - idx
                                                                          )
                                                            )
                                         );
                }
                else
                {
                    includeFiles.Add( inc );
                }
            }

            foreach ( string s in includeFiles )
            {
                log += s + "\n\t\t";
            }

            foreach ( string @ref in refs )
            {
                log += @ref + "\n\t\t";
            }

            ConsoleWriter.LogLine( log );

            for ( int i = refs.Count - 1; i >= 0; i-- )
            {
                string s = refs[i].Trim();

                if ( !File.Exists( s ) )
                {
                    ConsoleWriter.WarnLine( $"\t\tCan not find File: '{s}' Skipping.." );
                }
                else
                {
                    sb.AppendLine( File.ReadAllText( s ) );
                }
            }

            for ( int i = includeFiles.Count - 1; i >= 0; i-- )
            {
                string s = includeFiles[i];

                if ( !File.Exists( s ) )
                {
                    ConsoleWriter.WarnLine( $"\t\tCan not find File: '{s}' Skipping.." );
                }
                else
                {
                    sb.AppendLine( File.ReadAllText( s ) );
                }
            }

            return sb.ToString();
        }

        private static string ProcessGeneratedFile( ProjectSettings s, BuildTarget t, string src )
        {
            string outputFile = s.ResolveValue( "%Target.OutputFile%", t.Name );
            string outputFormat = s.ResolveValue( "%Target.OutputFormat%", t.Name );
            BuildOutputFormat fmt = ProjectSettings.GetOutputFormat( outputFormat );
            ConsoleWriter.LogLine( $"\tBuilding Output: {outputFile}" );
            ConsoleWriter.LogLine( $"\tOutput Format: {outputFormat}" );
            Directory.CreateDirectory( Path.GetDirectoryName( Path.GetFullPath( outputFile ) ) );
            fmt.BuildOutput( t, src, outputFile );

            return outputFile;
        }

        #endregion

    }

}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Console.Logging;
using BadScript.Profiling;

namespace BadScript.Console.Subsystems.Run
{

    public static class ScriptRunner
    {

        #region Public

        public static int Run( ScriptRunnerSettings settings )
        {
            BSProfilerData.EnableProfiler = settings.IsBenchmark;

            for ( int i = 0; i < settings.Iterations; i++ )
            {
                List < string > files = new List < string >();

                foreach ( string script in settings.Files )
                {
                    if ( Directory.Exists( script ) )
                    {
                        files.AddRange( Directory.GetFiles( script, "*.bs", SearchOption.AllDirectories ) );
                    }
                    else if ( File.Exists( script ) )
                    {
                        files.Add( script );
                    }
                    else
                    {
                        ConsoleWriter.ErrorLine( "Can not find File or Folder: " + script );
                    }
                }

                BSEngine engine = settings.CreateEngineSettings().Build();
                string[] args = settings.Arguments?.ToArray() ?? Array.Empty < string >();

                foreach ( string script in files )
                {
                    if ( !settings.NoLogo )
                    {
                        ConsoleWriter.LogLine( "Executing File: " + script );
                    }

                    engine.LoadFile( script, args, settings.IsBenchmark );
                }
            }

            if ( BSProfilerData.EnableProfiler )
            {
                string fName = $@"profiler_{DateTime.Now.Ticks}.json";

                string fDir = Path.Combine(
                                           BSConsoleDirectories.Instance.ProfilerDirectory,
                                           fName
                                          );

                if ( !settings.NoLogo )
                {
                    ConsoleWriter.LogLine( "Saving Profiler Output: " + fName );
                }

                string fData = BSProfilerData.SerializeProfilerData();
                File.WriteAllText( fDir, fData );
            }

            if ( !settings.NoLogo )
            {
                ConsoleWriter.SuccessLine( "Command 'run' finished!" );
            }

            return 0;
        }

        #endregion

    }

}

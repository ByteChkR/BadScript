using System.Collections.Generic;

using CommandLine;

namespace BadScript.Console.Subsystems.Run
{

    [Verb( "run", HelpText = "Runs one or multiple BS Script Files" )]
    public class ScriptRunnerSettings : EngineBuilderSettings
    {

        [Option( 'f', "files", Required = true, HelpText = "The File[s] or Directories that will be executed" )]
        public IEnumerable < string > Files { get; set; }

        [Option( 'a', "args", HelpText = "The start arguments that will be passed to the Files" )]
        public IEnumerable < string > Arguments { get; set; }

        [Option('b', "benchmark", HelpText = "Display the Execution time.")]
        public bool IsBenchmark { get; set; }
        [Option("iterations", HelpText = "How often do the scripts get executed?", Default = 1)]
        public int Iterations { get; set; }

    }

}

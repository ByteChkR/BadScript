using System.Collections.Generic;

using CommandLine;

namespace BadScript.Console.AppPackage
{

    [Verb( "app", HelpText = "Runs an App Package" )]
    public class AppPackageRunnerSettings : BSConsoleSettings
    {

        [Value( 0, HelpText = "Path to the app Package", Required = true )]
        public string AppPath { get; set; }

        [Option( 'a', "args" )]
        public IEnumerable < string > Arguments { get; set; }

    }

}

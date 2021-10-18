using System.Collections.Generic;

using CommandLine;

namespace BadScript.Console.AppPackage
{

    [Verb( "app", HelpText = "Runs an App Package" )]
    public class AppPackageRunnerSettings : BSConsoleSettings
    {

        [Value( 0, HelpText = "Path to the app Package", Required = true )]
        public string AppPath { get; set; }
        
        [Option('s', "secure", HelpText = "If specified the 'Environment' interface will be loaded as local variable, so no other loaded scripts will be able to access it.", Required = false)]
        public bool Secure { get; set; }

        [Option( 'a', "args" )]
        public IEnumerable < string > Arguments { get; set; }

    }

}

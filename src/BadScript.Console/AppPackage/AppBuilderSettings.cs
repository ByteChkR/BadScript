using System.Collections.Generic;

using CommandLine;

namespace BadScript.Console.AppPackage
{

    [Verb( "appbuilder", HelpText = "Builds an App Package" )]
    public class AppBuilderSettings : BSConsoleSettings
    {

        [Option( 'n', "name", HelpText = "App Name", Required = true )]
        public string AppName { get; set; }

        [Option( 'v', "version", HelpText = "App Version", Required = true )]
        public string AppVersion { get; set; }

        [Option( "minVer", HelpText = "Runtime Min Version", Default = null )]
        public string RuntimeMin { get; set; }

        [Option( "maxVer", HelpText = "Runtime Max Version", Default = null )]
        public string RuntimeMax { get; set; }

        [Option( 'f', "file", HelpText = "Source File", Required = true )]
        public string SourceFile { get; set; }

        [Option( 'o', "output", HelpText = "Output File", Default = "./app.bsapp" )]
        public string OutputFile { get; set; }

        [Option( 'r', "resources", HelpText = "Resource Path", Default = null )]
        public string ResourcePath { get; set; }

        [Option( 'i', "interfaces", HelpText = "Required and Interfaces", Default = null )]
        public IEnumerable < string > RequiredInterfaces { get; set; }

    }

}

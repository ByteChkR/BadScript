using CommandLine;

namespace BadScript.Console.Subsystems.Project
{

    [Verb(
             "make",
             HelpText =
                 "Builds a Project from the Information inside the build-settings.json file in the current working directory."
         )]
    public class ProjectBuilderSettings : BSConsoleSettings
    {

        [Option( 't', "target", HelpText = "The Build Target", Default = "build" )]
        public string BuildTarget { get; set; }

    }

}

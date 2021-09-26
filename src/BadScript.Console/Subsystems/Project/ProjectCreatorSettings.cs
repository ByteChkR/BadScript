using CommandLine;

namespace BadScript.Console.Subsystems.Project
{

    [Verb(
             "new",
             HelpText = "Creates a new Project Settings File based on a template in the current working directory"
         )]
    public class ProjectCreatorSettings : BSConsoleSettings
    {

        [Option(
                   't',
                   "template",
                   HelpText = "The Template that is used to create the new Project Settings File",
                   Default = "app"
               )]
        public string TemplateName { get; set; }

        [Option( 'n', "name", HelpText = "The name of the new Project" )]
        public string ProjectName { get; set; }

    }

}

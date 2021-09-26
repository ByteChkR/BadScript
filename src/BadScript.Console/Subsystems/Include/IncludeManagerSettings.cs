using CommandLine;

namespace BadScript.Console.Subsystems.Include
{

    [Verb( "include" )]
    public class IncludeManagerSettings : BSConsoleSettings
    {

        [Value( 0, Default = IncludeManagerOperations.List, HelpText = "The Include Operation(Add, Remove, List)" )]
        public IncludeManagerOperations Operation { get; set; }

        [Value( 1, HelpText = "File to Add/Remove" )]
        public string Target { get; set; }

    }

}

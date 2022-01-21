using CommandLine;

namespace BadScript.Console
{

    public abstract class BSConsoleSettings
    {

        [Option( "nologo", HelpText = "Does not print the BS Console Header if specified" )]
        public bool NoLogo { get; set; }

    }

}

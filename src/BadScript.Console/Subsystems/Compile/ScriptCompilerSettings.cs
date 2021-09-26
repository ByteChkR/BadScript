using CommandLine;

namespace BadScript.Console.Subsystems.Compile
{

    [Verb( "compile", HelpText = "Compiles a Script into a custom binary format." )]
    public class ScriptCompilerSettings : EngineBuilderSettings
    {

        [Option( 'i', "input", HelpText = "Input File that will be compiled." )]
        public string Input { get; set; }

        [Option( 'o', "output", HelpText = "Output File of the Compilation" )]
        public string Output { get; set; }

    }

}

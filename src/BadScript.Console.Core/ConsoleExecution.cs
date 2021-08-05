namespace BadScript.Console.Core
{

    internal struct ConsoleExecution
    {
        public readonly string OriginalFileName;
        public readonly string[] Arguments;
        public readonly bool IsBenchmark;
        private readonly BadScriptConsole Console;

        public ConsoleExecution( BadScriptConsole console, string file, string[] arguments, bool isBenchmark = false )
        {
            OriginalFileName = file;
            Arguments = arguments;
            IsBenchmark = isBenchmark;
            Console = console;
        }

        public string FileName => Console.FindScript( OriginalFileName.FixExtension() );

        public void Run( BSEngine engine )
        {
            engine.LoadFile( IsBenchmark, FileName, Arguments );
        }
    }

}

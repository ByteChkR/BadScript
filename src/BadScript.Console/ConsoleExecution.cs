namespace BadScript.Console
{

    internal struct ConsoleExecution
    {
        public readonly string OriginalFileName;
        public readonly string[] Arguments;
        public readonly bool IsBenchmark;

        public ConsoleExecution( string file, string[] arguments, bool isBenchmark = false )
        {
            OriginalFileName = file;
            Arguments = arguments;
            IsBenchmark = isBenchmark;
        }

        public string FileName => OriginalFileName.FixExtension().FindScript();

        public void Run( BSEngineInstance engine )
        {
            engine.LoadFile( IsBenchmark, FileName, Arguments );
        }
    }

}

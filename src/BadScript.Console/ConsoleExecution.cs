namespace BadScript.Console
{

    internal struct ConsoleExecution
    {
        public readonly string OriginalFileName;
        public readonly string[] Arguments;

        public ConsoleExecution(string file, string[] arguments)
        {
            OriginalFileName = file;
            Arguments = arguments;
        }

        public string FileName => OriginalFileName.FixExtension().FindScript();

        public void Run(BSEngineInstance engine)
        {
            engine.LoadFile(FileName, Arguments);
        }
    }

}
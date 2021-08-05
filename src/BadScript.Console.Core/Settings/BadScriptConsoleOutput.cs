using System;

namespace BadScript.Console.Core.Settings
{

    public struct BadScriptConsoleOutput
    {
        public static BadScriptConsoleOutput Default
        {
            get
            {
                BadScriptConsoleOutput outp = new BadScriptConsoleOutput();
                outp.OnClearConsole += System.Console.Clear;
                outp.OnWrite += System.Console.Write;
                outp.OnWriteLine += System.Console.WriteLine;
                outp.OnReadConsole += System.Console.ReadLine;

                return outp;
            }
        }

        public event Func < string > OnReadConsole;

        public event Action OnClearConsole;

        public event Action < string > OnWriteLine;

        public event Action < string > OnWrite;

        public void Clear()
        {
            OnClearConsole();
        }

        public void Write( string str )
        {
            OnWrite( str );
        }

        public void WriteLine( string str )
        {
            OnWriteLine( str );
        }

        public string Read()
        {
            return OnReadConsole();
        }
    }

}

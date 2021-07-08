using System.IO;
using BadScript.IO;

namespace BadScript.Console.IO
{

    public readonly struct ConsoleIORoot : IConsoleIORoot
    {
        public readonly string RootPath;

        public ConsoleIORoot( string rootPath )
        {
            RootPath = rootPath;
            Directory.CreateDirectory( RootPath );
        }
        public string GetRootPath()
        {
            return RootPath;
        }

        public int GetChildCount()
        {
            string[] fsEntries = Directory.GetFileSystemEntries( RootPath, "*", SearchOption.TopDirectoryOnly );
            return fsEntries.Length;
        }

        public IConsoleIOEntry GetChildAt( int index )
        {
            string[] fsEntries = Directory.GetFileSystemEntries(RootPath, "*", SearchOption.TopDirectoryOnly);

            if ( File.Exists( fsEntries[index] ) )
            {
                return new ConsoleIOFile( fsEntries[index], this, null );
            }
            return new ConsoleIODirectory(fsEntries[index], this, null);
        }
    }

}
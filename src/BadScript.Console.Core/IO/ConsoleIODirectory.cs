using System.IO;
using BadScript.IO;

namespace BadScript.Console.Core.IO
{

    public readonly struct ConsoleIODirectory : IConsoleIODirectory
    {
        public bool Exists => File.Exists( m_Path );

        private readonly string m_Name;
        private readonly IConsoleIORoot m_Root;
        private readonly IConsoleIODirectory m_Parent;
        private readonly string m_Path;

        public ConsoleIODirectory( string name, IConsoleIORoot root, IConsoleIODirectory parent )
        {
            m_Name = name;
            m_Root = root;
            m_Parent = parent;
            m_Path = "";
            m_Path = this.GetFullName();
        }

        public string GetName()
        {
            return m_Name;
        }

        public IConsoleIORoot GetRoot()
        {
            return m_Root;
        }

        public IConsoleIODirectory GetParent()
        {
            return m_Parent;
        }

        public int GetChildCount()
        {
            string[] files = Directory.GetFileSystemEntries( m_Path, "*", SearchOption.TopDirectoryOnly );

            return files.Length;
        }

        public IConsoleIOEntry GetChildAt( int index )
        {
            string[] files = Directory.GetFileSystemEntries( m_Path, "*", SearchOption.TopDirectoryOnly );

            if ( File.Exists( files[index] ) )
            {
                return new ConsoleIOFile( files[index], m_Root, m_Parent );
            }

            return new ConsoleIODirectory( files[index], m_Root, this );
        }
    }

}

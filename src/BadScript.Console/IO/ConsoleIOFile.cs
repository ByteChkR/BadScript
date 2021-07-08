using System.IO;
using BadScript.IO;

namespace BadScript.Console.IO
{

    public readonly struct ConsoleIOFile:IConsoleIOFile
    {
        private readonly string m_Name;
        private readonly IConsoleIORoot m_Root;
        private readonly IConsoleIODirectory m_Parent;
        private readonly string m_Path;
        public ConsoleIOFile( string name, IConsoleIORoot root, IConsoleIODirectory parent )
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

        public bool Exists => File.Exists( m_Path );

        public Stream Open()
        {
            return File.Open(m_Path, FileMode.OpenOrCreate);
        }
    }

}
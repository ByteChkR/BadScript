using System.IO;

namespace BadScript.IO
{

    public interface IConsoleIOFile : IConsoleIOEntry
    {
        Stream Open();
    }

}

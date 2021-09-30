using System.IO;

using BadScript.Utils;

namespace BadScript.Console.Subsystems
{

    public class EngineBuilderDirectories : SimpleSingleton < EngineBuilderDirectories >
    {

        public string IncludeDirectory => Path.Combine(BSConsoleDirectories.Instance.DataDirectory, "include");

        public EngineBuilderDirectories()
        {
            Directory.CreateDirectory( IncludeDirectory );
        }

    }

}

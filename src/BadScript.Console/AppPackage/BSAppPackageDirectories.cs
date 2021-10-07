using System.IO;

using BadScript.Utils;

namespace BadScript.Console.AppPackage
{

    public class BSAppPackageDirectories : SimpleSingleton < BSAppPackageDirectories >
    {

        public string AppDirectory => Path.Combine( BSConsoleDirectories.Instance.DataDirectory, "apps" );

        public string AppDataDirectory => Path.Combine( AppDirectory, "data" );

        public string AppPersistentDirectory => Path.Combine( AppDirectory, "persistent" );

        public string AppTempDirectory => Path.Combine( AppDirectory, "temp" );

    }

}

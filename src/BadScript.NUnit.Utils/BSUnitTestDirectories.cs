using System.IO;

using NUnit.Framework;

namespace BadScript.NUnit.Utils
{

    public static class BSUnitTestDirectories
    {

        public static string DataDirectory => Path.Combine( TestContext.CurrentContext.TestDirectory, "data" );

        public static string SettingsDirectory => Path.Combine( DataDirectory, "settings" );

    }

}

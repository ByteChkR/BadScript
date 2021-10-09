using System;
using System.IO;

using BadScript.Utils;

namespace BadScript.Console
{

    public class BSConsoleDirectories : SimpleSingleton < BSConsoleDirectories >
    {

        public string DataDirectory => Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "data" );

        public string SettingsDirectory => Path.Combine(DataDirectory, "settings");
        public string ProfilerDirectory => Path.Combine(DataDirectory, "profiler");

        #region Public

        public BSConsoleDirectories()
        {
            Directory.CreateDirectory(DataDirectory);
            Directory.CreateDirectory(ProfilerDirectory);
        }

        #endregion

    }

}

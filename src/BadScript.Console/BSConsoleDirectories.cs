using System;
using System.IO;

using BadScript.Settings;

namespace BadScript.Console
{

    public class BSConsoleDirectories : SimpleSingleton < BSConsoleDirectories >
    {

        public string DataDirectory => Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "data" );

        #region Public

        public BSConsoleDirectories()
        {
            Directory.CreateDirectory( DataDirectory );
        }

        #endregion

    }

}

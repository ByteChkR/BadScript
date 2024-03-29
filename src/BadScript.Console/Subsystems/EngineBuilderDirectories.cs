﻿using System.IO;

using BadScript.Utils;

namespace BadScript.Console.Subsystems
{

    public class EngineBuilderDirectories : SimpleSingleton < EngineBuilderDirectories >
    {

        public string IncludeDirectory => Path.Combine( BSConsoleDirectories.Instance.DataDirectory, "include" );

        #region Public

        public EngineBuilderDirectories()
        {
            Directory.CreateDirectory( IncludeDirectory );
        }

        #endregion

    }

}

using System;
using System.IO;

using BadScript.Utils;

namespace BadScript.Console
{

    public class BSConsoleDirectories : SimpleSingleton <BSConsoleDirectories>
    {

        public string DataDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
        

        public BSConsoleDirectories()
        {
            Directory.CreateDirectory( DataDirectory );
        }

    }


}

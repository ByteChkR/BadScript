using System;
using System.IO;

namespace BadScript.Console
{

    public static class BSConsoleResources
    {

        public static string DataDirectory => Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "data" );

    }

}

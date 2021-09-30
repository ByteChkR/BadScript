using System;
using System.IO;
using System.Numerics;

using BadScript.Console.Preprocessor;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.Settings;
using BadScript.StringUtils;
using BadScript.Utils;
using BadScript.Utils.Reflection;
using BadScript.Xml;
using BadScript.Zip;

namespace BadScript.Testing
{

    public static class Program
    {

        #region Public

        public static void Main( string[] args )
        {
            string file = "D:\\Users\\Tim\\Documents\\BadScript\\src\\BadScript.Testing\\PreprocessorTest.bs";

            System.Console.WriteLine( SourcePreprocessor.Preprocess( File.ReadAllText( file ), "DEBUG=true" ));

        }

        #endregion

    }

}

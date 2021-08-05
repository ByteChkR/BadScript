using System;
using BadScript.Common.Types.Implementations;
using BadScript.Console.Core;
using BadScript.Console.Core.Settings;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Utils;
using BadScript.Utils.Reflection;
using BadScript.Xml;
using BadScript.Zip;

namespace BadScript.Console
{

    public class Program
    {
        #region Public

        public static void Main( string[] args )
        {

            string root = AppDomain.CurrentDomain.BaseDirectory;

            BadScriptConsoleSettings settings = new BadScriptConsoleSettings(
                    root ).Add( new BadScriptCoreApi() ).
                           Add( new ConsoleColorApi() ).
                           Add( new BS2JsonInterface() ).
                           Add( new Json2BSInterface() ).
                           Add( new BSFileSystemInterface() ).
                           Add( new BSFileSystemPathInterface( root ) ).
                           Add( new BSFileSystemInterface() ).
                           Add( new BSMathApi() ).
                           Add( new HttpApi() ).
                           Add( new HttpServerApi() ).
                           Add( new ProcessApi() ).
                           Add( new StringUtilsApi() ).
                           Add( new ZipApi() ).
                           Add( new ImagingApi() ).
                           Add( new BSReflectionScriptInterface() ).
                           Add( new VersionToolsInterface() ).
                           Add( new XmlInterface() );

            settings.Add(
                new ConsoleApi(
                    x => settings.ConsoleOutput.Write( x.ToString() ),
                    x => settings.ConsoleOutput.WriteLine(
                        x.ToString() ),
                    settings.ConsoleOutput.Clear,
                    () => new BSObject(
                        settings.ConsoleOutput.Read() ) ) );

            BadScriptConsole console = new BadScriptConsole( settings );
            console.Run( args );
        }

        #endregion
    }

}

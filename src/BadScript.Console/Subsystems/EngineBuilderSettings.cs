using System;
using System.Collections.Generic;
using System.Linq;

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

using CommandLine;

namespace BadScript.Console.Subsystems
{

    public class EngineBuilderSettings : BSConsoleSettings
    {

        [Option( "include", Default = null, HelpText = "A list of Directories that will be loaded prior to the execution" )]
        public IEnumerable < string > IncludeDirectories { get; set; }

        [Option(
                   "interfaces",
                   HelpText =
                       "A list of Interfaces that will be loaded prior to the execution. Prefix an Interface with '#' to load it into global scope."
               )]
        public IEnumerable < string > ActiveInterfaces { get; set; }

        [Option( "optimize", Default = null, HelpText = "If specified the Parser will optimize the parsed output." )]
        public bool AllowOptimizations { get; set; }

        #region Public

        public BSEngineSettings CreateEngineSettings()
        {
            BSEngineSettings es =
                BSEngineSettings.MakeDefault( new BSParserSettings { AllowOptimization = AllowOptimizations } );

            es.Interfaces.Add( new BadScriptCoreApi() );
            es.Interfaces.Add( new ConsoleApi() );
            es.Interfaces.Add( new ConsoleColorApi() );
            es.Interfaces.Add( new BS2JsonInterface() );
            es.Interfaces.Add( new Json2BSInterface() );
            es.Interfaces.Add( new BSFileSystemInterface() );
            es.Interfaces.Add( new BSFileSystemPathInterface( AppDomain.CurrentDomain.BaseDirectory ) );
            es.Interfaces.Add( new BSMathApi() );
            es.Interfaces.Add( new HttpApi() );
            es.Interfaces.Add( new HttpServerApi() );
            es.Interfaces.Add( new ProcessApi() );
            es.Interfaces.Add( new StringUtilsApi() );
            es.Interfaces.Add( new ZipApi() );
            es.Interfaces.Add( new ImagingApi() );
            es.Interfaces.Add( new BSReflectionScriptInterface() );
            es.Interfaces.Add( new VersionToolsInterface() );
            es.Interfaces.Add( new XmlInterface() );

            string[] incDirs = IncludeDirectories.ToArray();
            if (incDirs .Length!=0 )
            {
                es.IncludeDirectories.AddRange(incDirs);
            }

            string[] interfaces = IncludeDirectories.ToArray();
            if (interfaces.Length!=0)
            {
                es.ActiveInterfaces.Clear();
                es.ActiveInterfaces.AddRange(interfaces);
            }

            return es;
        }

        #endregion

    }

}

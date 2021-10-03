using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.Interfaces.Versioning;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Parser;
using BadScript.Process;
using BadScript.Reflection;
using BadScript.StringUtils;
using BadScript.Xml;
using BadScript.Zip;

using CommandLine;

namespace BadScript.Console.Subsystems
{

    public class EngineBuilderSettings : BSConsoleSettings
    {

        [Option(
                   "include",
                   Default = null,
                   HelpText = "A list of Directories that will be loaded prior to the execution"
               )]
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

            es.Interfaces.Add( new BSCoreInterface() );
            es.Interfaces.Add( new BSConsoleInterface() );
            es.Interfaces.Add( new BSConsoleColorInterface() );
            es.Interfaces.Add( new BS2JsonInterface() );
            es.Interfaces.Add( new Json2BSInterface() );
            es.Interfaces.Add( new BSFileSystemInterface() );
            es.Interfaces.Add( new BSFileSystemPathInterface( AppDomain.CurrentDomain.BaseDirectory ) );
            es.Interfaces.Add( new BSMathInterface() );
            es.Interfaces.Add( new BSHttpInterface() );
            es.Interfaces.Add( new BSHttpServerInterface() );
            es.Interfaces.Add( new BSProcessInterface() );
            es.Interfaces.Add( new BSStringInterface() );
            es.Interfaces.Add( new BSZipInterface() );
            es.Interfaces.Add( new BSDrawingInterface() );
            es.Interfaces.Add( new BSVersioningInterface() );
            es.Interfaces.Add( new BSXmlInterface() );
            es.Interfaces.Add( BSReflectionInterface.Instance );

            string[] incDirs = IncludeDirectories.ToArray();

            if ( Directory.Exists( EngineBuilderDirectories.Instance.IncludeDirectory ) )
            {
                es.IncludeDirectories.Add( EngineBuilderDirectories.Instance.IncludeDirectory );
            }

            if ( incDirs.Length != 0 )
            {
                es.IncludeDirectories.AddRange( incDirs );
            }

            string[] interfaces = ActiveInterfaces.ToArray();

            if ( interfaces.Length != 0 )
            {
                es.ActiveInterfaces.Clear();
                es.ActiveInterfaces.AddRange( interfaces );
            }

            return es;
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.Interfaces.Versioning;
using BadScript.Reflection;
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

        #region Public

        public static BSEngineSettings CreateEngineSettings( string[] includeDirs, string[] activeInterfaces )
        {
            BSEngineSettings es =
                BSEngineSettings.MakeDefault();

            es.Interfaces.Add( new BSCollectionInterface() );
            es.Interfaces.Add( new BSConvertInterface() );
            es.Interfaces.Add( new BSVersioningInterface() );
            // es.Interfaces.Add( new BSCollectionInterface() );
            // es.Interfaces.Add( new BSConvertInterface() );
            // es.Interfaces.Add( new BSSystemConsoleInterface() );
            // es.Interfaces.Add( new BS2JsonInterface() );
            // es.Interfaces.Add( new Json2BSInterface() );
            // es.Interfaces.Add( new BSFileSystemInterface() );
            // es.Interfaces.Add( new BSFileSystemPathInterface( AppDomain.CurrentDomain.BaseDirectory ) );
            // es.Interfaces.Add( new BSMathInterface() );
            // es.Interfaces.Add( new BSHttpInterface() );
            // es.Interfaces.Add( new BSHttpServerInterface() );
            // es.Interfaces.Add( new BSProcessInterface() );
            // es.Interfaces.Add( new BSStringInterface() );
            // es.Interfaces.Add( new BSZipInterface() );
            // es.Interfaces.Add( new BSDrawingInterface() );
            // es.Interfaces.Add( new BSVersioningInterface() );
            // es.Interfaces.Add(new BSXmlInterface());
            //es.Interfaces.Add(new BSThreadingInterface());
            es.Interfaces.Add( BSReflectionInterface.Instance );

            if ( Directory.Exists( EngineBuilderDirectories.Instance.IncludeDirectory ) )
            {
                es.IncludeDirectories.Add( EngineBuilderDirectories.Instance.IncludeDirectory );
            }

            if ( includeDirs.Length != 0 )
            {
                es.IncludeDirectories.AddRange( includeDirs );
            }

            if ( activeInterfaces.Length != 0 )
            {
                es.ActiveInterfaces.Clear();
                es.ActiveInterfaces.AddRange( activeInterfaces );
            }

            return es;
        }

        public BSEngineSettings CreateEngineSettings()
        {
            return CreateEngineSettings( IncludeDirectories.ToArray(), ActiveInterfaces.ToArray() );
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BadScript.Console.Core.IO;
using BadScript.Interfaces;
using BadScript.IO;
using BadScript.Settings;

namespace BadScript.Console.Core.Settings
{

    public struct BadScriptConsoleSettings
    {
        public bool AddEnvironmentApi;
        public readonly string AppRoot;
        public readonly ConsoleIORoot DataRoot;
        public readonly ConsoleIODirectory AppDirectory;
        public readonly ConsoleIODirectory IncludeDirectory;
        public readonly BSEngineSettings EngineSettings;

        public BadScriptConsoleOutput ConsoleOutput { get; private set; }

        private Dictionary < string, Version > m_VersionTags;

        public List < (string, Version) > GetVersions()
        {
            return m_VersionTags.Select( x => ( x.Key, x.Value ) ).ToList();
        }

        public BadScriptConsoleSettings( string appRoot )
        {
            AppRoot = appRoot;
            AddEnvironmentApi = true;
            ConsoleOutput = BadScriptConsoleOutput.Default;

            DataRoot =
                new( Path.Combine( AppRoot, "bs-data/" ) );

            AppDirectory = new( "apps", DataRoot, null );
            IncludeDirectory = new( "include", DataRoot, null );

            m_VersionTags = new Dictionary < string, Version >();

            EngineSettings = null;
            EngineSettings = BSEngineSettings.MakeDefault( LoadParserSettings(), LoadRuntimeSettings() );

            AppDirectory.EnsureExistsSelf();
            IncludeDirectory.EnsureExistsSelf();
        }

        public BadScriptConsoleSettings SetOutput( BadScriptConsoleOutput outp )
        {
            ConsoleOutput = outp;

            return this;
        }

        public BadScriptConsoleSettings Add( ABSScriptInterface api )
        {
            EngineSettings.Interfaces.Add( api );
            Assembly asm = api.GetType().Assembly;
            AssemblyName name = asm.GetName();

            if ( m_VersionTags.ContainsKey( name.Name ) )
            {
                m_VersionTags[name.Name] = name.Version;
            }

            return this;
        }

        private BSParserSettings LoadParserSettings()
        {
            IConsoleIOFile configFile = new ConsoleIOFile( "parser.json", DataRoot, null );
            configFile.EnsureExistParent();

            if ( configFile.Exists )
            {
                return configFile.ParseJson < BSParserSettings >();
            }
            else
            {
                configFile.WriteJson( BSParserSettings.Default );

                return BSParserSettings.Default;
            }
        }

        private BSRuntimeSettings LoadRuntimeSettings()
        {
            IConsoleIOFile configFile = new ConsoleIOFile( "runtime.json", DataRoot, null );
            configFile.EnsureExistParent();

            if ( configFile.Exists )
            {
                return configFile.ParseJson < BSRuntimeSettings >();
            }
            else
            {
                configFile.WriteJson( BSRuntimeSettings.Default );

                return BSRuntimeSettings.Default;
            }
        }
    }

}

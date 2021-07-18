using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Console.IO;
using BadScript.IO;

namespace BadScript.Console.Plugins
{

    public class PluginLoader
    {
        private readonly IConsoleIORoot m_Root;

        private readonly IConsoleIODirectory m_PluginDirectory;
        private readonly IConsoleIOFile m_PluginIndexFile;

        private readonly IConsoleIODirectory m_PluginAssemblyDirectory;

        #region Public

        public PluginLoader( IConsoleIORoot root, IConsoleIODirectory pluginDirectory )
        {
            m_Root = root;
            m_PluginDirectory = pluginDirectory;
            m_PluginIndexFile = new ConsoleIOFile( "active-plugins.json", root, m_PluginDirectory );
            m_PluginIndexFile.EnsureExistParent();

            if ( !m_PluginIndexFile.Exists )
            {
                m_PluginIndexFile.WriteJson( new string[0] );
            }

            m_PluginAssemblyDirectory = new ConsoleIODirectory( "plugins", m_Root, m_PluginDirectory );
            m_PluginAssemblyDirectory.EnsureExistsSelf();
        }

        public void LoadPlugins()
        {
            BSEngineInstance scriptInstance = null;
            string[] activePlugins = m_PluginIndexFile.ParseJson < string[] >();
            Dictionary < string, ABSObject > loadedScripts = new Dictionary < string, ABSObject >();

            foreach ( string activePlugin in activePlugins )
            {
                IConsoleIOFile pluginFile = new ConsoleIOFile(
                    activePlugin,
                    m_Root,
                    m_PluginAssemblyDirectory );

                if ( pluginFile.Exists )
                {
                    string fullName = pluginFile.GetFullName();

                    if ( fullName.EndsWith( ".dll" ) )
                    {
                        try
                        {
                            Assembly asm = Assembly.LoadFrom( fullName );
                            LoadAssembly( asm );
                        }
                        catch ( Exception e )
                        {
                            throw new Exception( "Unable to load Plugin: " + pluginFile.GetFullName(), e );
                        }
                    }
                    else if ( fullName.EndsWith( ".bs" ) )
                    {
                        if ( scriptInstance == null )
                        {
                            scriptInstance = BSEngine.CreateEngineInstance( BSEngine.GetDefaultInterfaces() );
                        }

                        ABSObject o = scriptInstance.
                            LoadFile( fullName, new string[0] );

                        loadedScripts[Path.GetFileNameWithoutExtension( fullName )] = o;
                    }
                }
            }

            BSEngine.AddStatic(
                new BSConstantScriptInterface(
                    "plugins",
                    x =>
                    {
                        foreach ( KeyValuePair < string, ABSObject > loadedScript in loadedScripts )
                        {
                            x.InsertElement(
                                new BSObject( loadedScript.Key ),
                                loadedScript.Value );
                        }
                    } ) );
        }

        #endregion

        #region Private

        private void LoadAssembly( Assembly asm )
        {
            Type[] types = asm.GetExportedTypes();

            foreach ( Type type in types )
            {
                RuntimeHelpers.RunClassConstructor( type.TypeHandle );
            }
        }

        #endregion
    }

}

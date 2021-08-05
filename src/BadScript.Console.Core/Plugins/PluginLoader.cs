using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using BadScript.Console.Core.IO;
using BadScript.IO;

namespace BadScript.Console.Core.Plugins
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
            BSEngine scriptInstance = null;
            string[] activePlugins = m_PluginIndexFile.ParseJson < string[] >();

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
                }
            }
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BadScript.Plugins
{

    public static class PluginManager
    {

        private static readonly List < Plugin > m_Plugins = new List < Plugin >();

        private static readonly Dictionary < object, List < Plugin > > m_LoadedPlugins =
            new Dictionary < object, List < Plugin > >();

        public static event Action < object > OnLog;

        public static event Action < object > OnError;

        public static IEnumerable < Plugin > Plugins => m_Plugins;

        #region Public

        public static void InitializePlugins( string path, bool log )
        {
            string[] files = Directory.GetFiles( path, "*.dll", SearchOption.AllDirectories );
            StringBuilder sb = new StringBuilder( "Loaded Plugins: " );

            foreach ( string file in files )
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom( file );

                    foreach ( Plugin plugin in assembly.GetPlugins() )
                    {
                        sb.Append( "\n\t" + plugin );
                        m_Plugins.Add( plugin );
                    }
                }
                catch ( Exception e )
                {
                    OnError?.Invoke( $"Loading '{file}' failed: " + e.Message );
                }
            }

            if(log)
                OnLog?.Invoke( sb.ToString() );
        }

        public static IEnumerable < Plugin > LoadedPlugins( object i )
        {
            return m_LoadedPlugins.ContainsKey( i ) ? m_LoadedPlugins[i] : Enumerable.Empty < Plugin >();
        }

        public static void LoadPlugins( object item )
        {
            List < Plugin > plugins = m_LoadedPlugins.ContainsKey( item )
                                          ? m_LoadedPlugins[item]
                                          : m_LoadedPlugins[item] = new List < Plugin >();

            foreach ( Plugin mPlugin in m_Plugins )
            {
                if ( mPlugin.Load( item ) )
                {
                    plugins.Add( mPlugin );
                }
            }
        }

        #endregion

    }

}

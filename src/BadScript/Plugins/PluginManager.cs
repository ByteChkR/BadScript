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
        public static event Action<object> OnLog;
        public static event Action<object> OnError;
        private static readonly List<Plugin> m_Plugins = new List<Plugin>();
        private static readonly Dictionary<object, List<Plugin>> m_LoadedPlugins = new Dictionary<object, List<Plugin>>();
        
        public static IEnumerable<Plugin> Plugins => m_Plugins;
        public static IEnumerable<Plugin> LoadedPlugins(object i) => m_LoadedPlugins.ContainsKey(i) ? m_LoadedPlugins[i] : Enumerable.Empty<Plugin>();
        

        public static void InitializePlugins(string path)
        {
            var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            StringBuilder sb = new StringBuilder("Loaded Plugins: ");
            foreach (var file in files)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var types = assembly.GetTypes();
                    foreach (Plugin plugin in assembly.GetPlugins())
                    {
                        
                        sb.Append("\n\t"+plugin);
                        m_Plugins.Add(plugin);
                    }
                }
                catch (Exception e)
                {
                    OnError?.Invoke($"Loading '{file}' failed: " + e.Message);
                }
            }
            
            OnLog?.Invoke(sb.ToString());
        }


        public static void LoadPlugins(object item)
        {
            List<Plugin> plugins = m_LoadedPlugins.ContainsKey(item)
                ? m_LoadedPlugins[item]
                : (m_LoadedPlugins[item] = new List<Plugin>());
            
            foreach (Plugin mPlugin in m_Plugins)
            {
                if (mPlugin.Load(item))
                {
                    plugins.Add(mPlugin);
                }
            }
        }
    }
}
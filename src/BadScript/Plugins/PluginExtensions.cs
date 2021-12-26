using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BadScript.Plugins
{
    public static class PluginExtensions
    {
        
        public static IEnumerable<Type> GetPluginTypes<T>(this Assembly asm)
        {
            return asm.GetTypes().Where(t => typeof(Plugin<T>).IsAssignableFrom(t));
        }

        public static IEnumerable<Plugin<T>> GetPlugins<T>(this Assembly asm)
        {
            return asm.GetPluginTypes<T>().Select(x => (Plugin<T>) Activator.CreateInstance(x));
        }
        public static IEnumerable<Type> GetPluginTypes(this Assembly asm)
        {
            return asm.GetTypes().Where(t => typeof(Plugin).IsAssignableFrom(t));
        }

        public static IEnumerable<Plugin> GetPlugins(this Assembly asm)
        {
            return asm.GetPluginTypes().Select(x => (Plugin) Activator.CreateInstance(x));
        }

    }
}
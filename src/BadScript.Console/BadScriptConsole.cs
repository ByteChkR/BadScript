using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Console.IO;
using BadScript.Console.Plugins;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.Https;
using BadScript.Imaging;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Zip;

namespace BadScript.Console
{

    internal class BadScriptConsole
    {
        private static PluginLoader m_PluginLoader;

        private static readonly ConsoleIORoot m_IORoot =
            new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bs-data/"));

        public static readonly ConsoleIODirectory AppDirectory = new("apps", m_IORoot, null);
        public static readonly ConsoleIODirectory IncludeDirectory = new("include", m_IORoot, null);

        #region Private

        private static string GetConsoleIncludeDir()
        {
            IncludeDirectory.EnsureExistsSelf();
            return IncludeDirectory.GetFullName();
        }

        private static string[] GetDefaultInterfaces()
        {
            IConsoleIOFile configFile = new ConsoleIOFile("interfaces.json", m_IORoot, null);
            configFile.EnsureExistParent();

            if (configFile.Exists)
            {
                var ret = configFile.ParseJson<string[]>();

                return ret;
            }
            else
            {
                string[] ret = {"#core", "#console"};
                configFile.WriteJson(ret);

                return ret;
            }
        }

        private static string LoadConfig(BSEngineInstance i, string src)
        {
            return i.LoadFile(src, new string[0]).ConvertString();
        }


        private static void Main(string[] args)
        {
            m_PluginLoader = new PluginLoader(m_IORoot, new ConsoleIODirectory("plugins", m_IORoot, null));

            m_PluginLoader.LoadPlugins();
            BSEngine.AddStatic(new BS2JsonInterface());
            BSEngine.AddStatic(new BSFileSystemInterface());
            BSEngine.AddStatic(new BSFileSystemPathInterface(AppDomain.CurrentDomain.BaseDirectory));
            BSEngine.AddStatic(new BadScriptCoreApi());
            BSEngine.AddStatic(new BSMathApi());
            BSEngine.AddStatic(new ConsoleApi());
            BSEngine.AddStatic(new ConsoleColorApi());
            BSEngine.AddStatic(new HttpApi());
            BSEngine.AddStatic(new HttpServerApi());
            BSEngine.AddStatic(new Json2BSInterface());
            BSEngine.AddStatic(new ProcessApi());
            BSEngine.AddStatic(new StringUtilsApi());
            BSEngine.AddStatic(new ZipApi());
            BSEngine.AddStatic(new ImagingApi());

            AppDirectory.EnsureExistsSelf();

            var engine = BSEngine.CreateEngineInstance(GetDefaultInterfaces(), GetConsoleIncludeDir());

            var a = "";

            foreach (var s in args) a += " " + s;

            var ar = a.Split(';');

            var executions = new List<ConsoleExecution>();
            foreach (var execStr in ar)
            {
                var parts = execStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                executions.Add(new ConsoleExecution(parts[0], parts.Skip(1).ToArray()));
            }

            foreach (var consoleExecution in executions) consoleExecution.Run(engine);

            /*foreach ( string s in ar )
            {
                string[] parts = s.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
                engine.LoadFile( parts[0], parts.Skip( 1 ).ToArray() );
            }*/
        }

        #endregion
    }
}
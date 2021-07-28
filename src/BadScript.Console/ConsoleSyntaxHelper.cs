using System.IO;
using BadScript.IO;

namespace BadScript.Console
{

    internal static class ConsoleSyntaxHelper
    {
        public static string FixExtension(this string t)
        {
            var ext = Path.GetExtension(t);
            if (string.IsNullOrEmpty(ext)) return t + ".bs";
            return t;
        }

        public static string FindScript(this string t)
        {
            if (File.Exists(t))
                return t;
            var p = Path.Combine(BadScriptConsole.AppDirectory.GetFullName(), t);
            if (File.Exists(p))
                return p;
            return t;
        }
    }

}
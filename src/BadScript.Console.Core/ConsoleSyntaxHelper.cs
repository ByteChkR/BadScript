using System.IO;
using BadScript.IO;

namespace BadScript.Console.Core
{

    internal static class ConsoleSyntaxHelper
    {
        #region Public

        public static string FindScript( this BadScriptConsole cli, string t )
        {
            if ( File.Exists( t ) )
            {
                return t;
            }

            string p = Path.Combine( cli.Settings.AppDirectory.GetFullName(), t );

            if ( File.Exists( p ) )
            {
                return p;
            }

            return t;
        }

        public static string FixExtension( this string t )
        {
            string? ext = Path.GetExtension( t );

            if ( string.IsNullOrEmpty( ext ) )
            {
                return t + ".bs";
            }

            return t;
        }

        #endregion
    }

}

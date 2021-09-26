using System;

namespace BadScript.Console.Subsystems
{

    public static class ConsoleWriter
    {

        public static LogColor LogColor = new LogColor( ConsoleColor.Magenta );
        public static LogColor SuccessColor = new LogColor( ConsoleColor.Green );
        public static LogColor WarningColor = new LogColor( ConsoleColor.DarkYellow );
        public static LogColor ErrorColor = new LogColor( ConsoleColor.Red );

        #region Public

        public static void Error( object o )
        {
            ErrorColor.Write( o );
        }

        public static void ErrorLine( object o )
        {
            ErrorColor.WriteLine( o );
        }

        public static void Log( object o )
        {
            LogColor.Write( o );
        }

        public static void LogLine( object o )
        {
            LogColor.WriteLine( o );
        }

        public static void Success( object o )
        {
            SuccessColor.Write( o );
        }

        public static void SuccessLine( object o )
        {
            SuccessColor.WriteLine( o );
        }

        public static void Warn( object o )
        {
            WarningColor.Write( o );
        }

        public static void WarnLine( object o )
        {
            WarningColor.WriteLine( o );
        }

        #endregion

    }

}

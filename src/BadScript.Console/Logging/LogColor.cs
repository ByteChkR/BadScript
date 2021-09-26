using System;

namespace BadScript.Console.Logging
{

    public struct LogColor
    {

        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;

        public LogColor( ConsoleColor fg, ConsoleColor bg = ConsoleColor.Black )
        {
            ForeColor = fg;
            BackColor = bg;
        }

        public void Write( object o )
        {
            if ( o == null )
            {
                return;
            }

            ConsoleColor fg = System.Console.ForegroundColor;
            ConsoleColor bg = System.Console.BackgroundColor;
            System.Console.ForegroundColor = ForeColor;
            System.Console.BackgroundColor = BackColor;
            System.Console.Write( o.ToString() );
            System.Console.ForegroundColor = fg;
            System.Console.BackgroundColor = bg;
        }

        public void WriteLine( object o )
        {
            if ( o == null )
            {
                return;
            }

            ConsoleColor fg = System.Console.ForegroundColor;
            ConsoleColor bg = System.Console.BackgroundColor;
            System.Console.ForegroundColor = ForeColor;
            System.Console.BackgroundColor = BackColor;
            System.Console.WriteLine( o.ToString() );
            System.Console.ForegroundColor = fg;
            System.Console.BackgroundColor = bg;
        }

    }

}

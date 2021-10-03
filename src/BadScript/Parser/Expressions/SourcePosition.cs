using System;
using System.Linq;

namespace BadScript.Parser.Expressions
{

    public struct SourcePosition
    {

        public string Source;
        public string LineStr;
        public int Line;
        public int Collumn;
        public int Position;

        public static SourcePosition Unknown =>
            new SourcePosition
            {
                Source = "UNKNOWN",
                LineStr = "UNKNOWN",
                Line = 0,
                Collumn = 0,
                Position = 0
            };

        public static SourcePosition GetCurrentLineInfo( string src, int pos )
        {
            char[] text = src.ToCharArray();

            if ( pos < src.Length )
            {
                text = text.Take( pos ).ToArray();
            }

            int lineCount = 0;
            int lastNewLine = 0;

            for ( int i = 0; i < pos; i++ )
            {
                if ( text[i] == '\n' )
                {
                    lastNewLine = i;
                    lineCount++;
                }
            }

            int textStart = lastNewLine + 1;
            int nextNewLine = src.IndexOf( '\n', textStart );

            if ( nextNewLine == -1 )
            {
                nextNewLine = src.Length - textStart;
            }

            string r = src.Substring( textStart, Math.Min( nextNewLine, src.Length - textStart ) );

            return new SourcePosition
                   {
                       Source = src,
                       LineStr = r.Trim(),
                       Line = lineCount,
                       Collumn = pos - lastNewLine,
                       Position = pos
                   };
        }

    }

}

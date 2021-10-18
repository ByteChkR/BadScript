using System;
using System.Linq;

namespace BadScript.Parser.Expressions
{

    public readonly struct SourcePosition
    {

        public readonly string Source;
        public readonly string LineStr;
        public readonly int Line;
        public readonly int Collumn;
        public readonly int Position;

        private SourcePosition( string src, string line, int lineNum, int col, int pos )
        {
            Source = src;
            LineStr = line;
            Line = lineNum;
            Collumn = col;
            Position = pos;
        }

        public static readonly SourcePosition Unknown =
            new SourcePosition(
                               "UNKNOWN",
                               "UNKNOWN",
                               0,
                               0,
                               0
                              );

        public static SourcePosition GetCurrentLineInfo( string src, int pos )
        {
            char[] text = src.ToCharArray();

            if ( pos < src.Length )
            {
                text = text.Take( pos ).ToArray();
            }
            else
            {
                pos = src.Length;
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

            return new SourcePosition(
                                      src,
                                      r.Trim(),
                                      lineCount,
                                      pos - lastNewLine,
                                      pos
                                     );
        }

    }

}

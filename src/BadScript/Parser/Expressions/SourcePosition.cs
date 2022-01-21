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

        private static int CountLines( string src, int pos )
        {
            int lines = 0;

            for ( int i = 0; i < pos; i++ )
            {
                if ( i >= src.Length )
                {
                    break;
                }

                if ( src[i] == '\n' )
                {
                    lines++;
                }
            }

            return lines + 1;
        }

        private static int GetLineIndex( string src, int pos )
        {
            int index = 0;

            for ( int i = pos; i > 0; i-- )
            {
                if ( i >= src.Length )
                {
                    continue;
                }

                if ( src[i] == '\n' )
                {
                    index = i;

                    break;
                }
            }

            if ( index != 0 )
            {
                return index + 1;
            }

            return index;
        }

        public static SourcePosition GetSourcePosition( string src, int pos )
        {
            int lineCount = CountLines( src, pos );
            int lineIndex = GetLineIndex( src, pos );

            int start = pos;
            char[] str = src.ToCharArray();
            int nextLine = src.IndexOf( '\n', start + 1 );

            if ( nextLine == -1 )
            {
                nextLine = src.Length;
            }

            string line = src.Substring( lineIndex, nextLine - lineIndex );
            int col = start - lineIndex;

            return new SourcePosition( src, line, lineCount, col, pos );
        }

        //public static SourcePosition GetCurrentLineInfo( string src, int pos )
        //{
        //    return GetSourcePosition( src, pos );
        //    char[] text = src.ToCharArray();

        //    if ( pos < src.Length )
        //    {
        //        text = text.Take( pos ).ToArray();
        //    }
        //    else
        //    {
        //        pos = src.Length;
        //    }

        //    int lineCount = 0;
        //    int lastNewLine = 0;

        //    for ( int i = 0; i < pos; i++ )
        //    {
        //        if ( text[i] == '\n' )
        //        {
        //            lastNewLine = i;
        //            lineCount++;
        //        }
        //    }

        //    int textStart = lastNewLine + 1;
        //    int nextNewLine = src.IndexOf( '\n', textStart );

        //    if ( nextNewLine == -1 )
        //    {
        //        nextNewLine = src.Length - textStart;
        //    }

        //    string r = src.Substring( textStart, Math.Min( nextNewLine, src.Length - textStart ) );

        //    return new SourcePosition(
        //                              src,
        //                              r.Trim(),
        //                              lineCount,
        //                              pos - lastNewLine,
        //                              pos
        //                             );
        //}

    }

}

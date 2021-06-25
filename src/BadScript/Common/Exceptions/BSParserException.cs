using System;

namespace BadScript.Common.Exceptions
{

    public class BSParserException : Exception
    {
        #region Public

        public BSParserException( string msg ) : base( msg )
        {
        }

        public BSParserException( string msg, BSParser parser ) : base( GenerateErrorMessage( msg, parser ) )
        {
        }

        #endregion

        #region Private

        private static string GenerateErrorMessage( string msg, BSParser parser )
        {
            ( string line, int lineCount, int col ) = parser.GetCurrentLineInfo();

            return $"Parser Exception: '{msg}' at {lineCount}:{col}\nLine: '{line}'";
        }

        #endregion
    }

}

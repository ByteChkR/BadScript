using System;

using BadScript.Parser;
using BadScript.Parser.Expressions;

namespace BadScript.Exceptions
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
            SourcePosition p = parser.CreateSourcePosition();

            return $"Parser Exception: '{msg}' at {p.Line}:{p.Collumn}\nLine: '{p.LineStr}'";
        }

        #endregion

    }

}

using System;
using BadScript.Common.Expressions;

namespace BadScript.Common.Exceptions
{

    public class BSRuntimeException : Exception
    {
        public readonly SourcePosition Position;

        #region Public

        public BSRuntimeException( string msg ) : base( msg )
        {
            Position = SourcePosition.Unknown;
        }

        public BSRuntimeException( SourcePosition pos, string msg ) : base( GenerateMessage( pos, msg ) )
        {
            Position = pos;
        }

        #endregion

        #region Private

        private static string GenerateMessage( SourcePosition p, string msg )
        {
            return $"Runtime Exception: '{msg}' at {p.Line}:{p.Collumn}\nLine: '{p.LineStr}'";

        }

        #endregion
    }

}

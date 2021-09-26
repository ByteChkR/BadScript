using System;

using BadScript.Common.Expressions;
using BadScript.Common.Types;

namespace BadScript.Common.Exceptions
{

    public class BSRuntimeException : Exception
    {

        public SourcePosition Position { get; }

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
            return $"Runtime Exception: '{msg}' at {p.Line}:{p.Collumn}\nLine: '{p.LineStr}'\n{BSFunction.FlatTrace}";
        }

        #endregion

    }

}

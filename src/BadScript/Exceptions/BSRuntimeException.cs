using System;

using BadScript.Parser.Expressions;
using BadScript.Types;

namespace BadScript.Exceptions
{

    public class BSRuntimeException : Exception
    {

        public SourcePosition Position { get; }

        #region Public

        public BSRuntimeException( string msg ) : base( msg )
        {
            Position = SourcePosition.Unknown;
        }

        public BSRuntimeException(string msg, Exception innerException) : base(msg, innerException)
        {
            Position = SourcePosition.Unknown;
        }
        public BSRuntimeException(SourcePosition pos, string msg) : base(GenerateMessage(pos, msg))
        {
            Position = pos;
        }

        public BSRuntimeException(SourcePosition pos, string msg, Exception innerException) : base(GenerateMessage(pos, msg), innerException)
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

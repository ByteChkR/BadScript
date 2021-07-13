using BadScript.Common.Expressions;
using BadScript.Common.Types;

namespace BadScript.Common.Exceptions
{

    public class BSInvalidOperationException : BSRuntimeException
    {
        #region Public

        public BSInvalidOperationException( SourcePosition pos, string op, params ABSObject[] o ) : base(
            pos,
            GenerateOperationErrorText( op, o ) )
        {
        }

        #endregion

        #region Private

        private static string GenerateOperationErrorText( string op, ABSObject[] o )
        {
            string r = $"Can not apply '{op}' between objects: ";

            for ( int i = 0; i < o.Length; i++ )
            {
                ABSObject absObject = o[i];

                if ( i != o.Length - 1 )
                {
                    r += r + ", ";
                }

                r += absObject;
            }

            return r;
        }

        #endregion
    }

}

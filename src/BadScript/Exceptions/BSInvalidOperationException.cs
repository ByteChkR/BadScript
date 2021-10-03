using System.Text;

using BadScript.Parser.Expressions;
using BadScript.Types;

namespace BadScript.Exceptions
{

    public class BSInvalidOperationException : BSRuntimeException
    {

        #region Public

        public BSInvalidOperationException( SourcePosition pos, string op, params ABSObject[] o ) : base(
             pos,
             GenerateOperationErrorText( op, o )
            )
        {
        }

        #endregion

        #region Private

        private static string GenerateOperationErrorText( string op, ABSObject[] o )
        {
            StringBuilder sb = new StringBuilder( "Can not apply '" );
            sb.Append( op );
            sb.Append( "' between objects: " );

            for ( int i = 0; i < o.Length; i++ )
            {
                ABSObject absObject = o[i];

                if ( i != o.Length - 1 )
                {
                    sb.Append( ", " );
                }

                sb.Append( absObject );
            }

            return sb.ToString();
        }

        #endregion

    }

}

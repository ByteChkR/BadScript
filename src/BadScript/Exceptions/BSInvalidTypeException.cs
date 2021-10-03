using System.Text;

using BadScript.Parser.Expressions;
using BadScript.Types;

namespace BadScript.Exceptions
{

    public class BSInvalidTypeException : BSRuntimeException
    {

        #region Public

        public BSInvalidTypeException(
            SourcePosition pos,
            string msg,
            ABSObject o,
            params string[] acceptedTypes ) : base(
                                                   pos,
                                                   GenerateRuntimeMessage( msg, o, acceptedTypes )
                                                  )
        {
        }

        #endregion

        #region Private

        private static string GenerateRuntimeMessage( string msg, ABSObject o, string[] acceptedTypes )
        {
            StringBuilder sb = new StringBuilder( "Runtime Exception: '" );
            sb.Append( msg );
            sb.Append( "'\nInvalid Object: " );
            sb.Append( o );

            foreach ( string acceptedType in acceptedTypes )
            {
                sb.Append( "\nAccepted Type: " );
                sb.Append( acceptedType );
            }

            return sb.ToString();
        }

        #endregion

    }

}

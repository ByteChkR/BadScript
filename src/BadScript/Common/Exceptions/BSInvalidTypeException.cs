using BadScript.Common.Expressions;
using BadScript.Common.Types;

namespace BadScript.Common.Exceptions
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
            string r = $"Runtime Exception: '{msg}'\nInvalid Object: {o}";

            foreach ( string acceptedType in acceptedTypes )
            {
                r += $"\nAccepted Type: {acceptedType}";
            }

            return r;
        }

        #endregion

    }

}

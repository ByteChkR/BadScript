using BadScript.Common.Types;

namespace BadScript.Common.Exceptions
{

    public class BSInvalidTypeException : BSRuntimeException
    {
        #region Public

        public BSInvalidTypeException( string msg, ABSObject o, params string[] acceptedTypes ) : base(
            GenerateRuntimeMessage( msg, o, acceptedTypes ) )
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

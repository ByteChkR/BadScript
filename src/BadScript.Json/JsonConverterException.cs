using BadScript.Exceptions;

namespace BadScript.Json
{

    internal class JsonConverterException : BSRuntimeException
    {

        #region Public

        public JsonConverterException( string msg ) : base( msg )
        {
        }

        #endregion

    }

}

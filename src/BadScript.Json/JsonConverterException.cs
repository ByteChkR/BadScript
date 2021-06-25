using BadScript.Common.Exceptions;

namespace BadScript.Json
{

    internal class JsonConverterException : BSRuntimeException
    {
        public JsonConverterException(string msg ) : base( msg )
        {
        }
    }

}
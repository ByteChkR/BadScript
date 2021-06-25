using System;

namespace BadScript.Common.Exceptions
{

    public class BSRuntimeException : Exception
    {
        public BSRuntimeException(string msg) : base(msg)
        {

        }
    }

}
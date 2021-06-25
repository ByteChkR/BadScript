using System;

namespace BadScript.Common.Exceptions
{

    public class BSRuntimeException : Exception
    {
        #region Public

        public BSRuntimeException( string msg ) : base( msg )
        {

        }

        #endregion
    }

}

using System;

namespace BadScript.Tools.CodeGenerator.Runtime.Attributes
{

    public abstract class WrapperObjectCreator < T > : IWrapperObjectCreator
        where T : new()
    {

        public int ArgCount => 0;

        #region Public

        public object Create( object[] args )
        {
            if ( args.Length != 0 )
            {
                throw new Exception( $"Invalid argument count in constructor of wrapped type '{typeof( T )}'." );
            }

            return new T();
        }

        #endregion

    }

}

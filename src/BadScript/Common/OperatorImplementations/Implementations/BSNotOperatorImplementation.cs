using System;

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSNotOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public BSNotOperatorImplementation() : base( "!" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];

            return lVal.TryConvertBool( out bool _ );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];

            if ( lVal.TryConvertBool( out bool lD ) )
            {
                return new BSObject( ( decimal ) ( lD ? 1 : 0 ) );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        #endregion

    }

}

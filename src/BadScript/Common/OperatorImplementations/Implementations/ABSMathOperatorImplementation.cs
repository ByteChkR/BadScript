using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public abstract class ABSMathOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            return lVal.TryConvertDecimal( out decimal _ ) && rVal.TryConvertDecimal( out decimal _ );
        }

        #endregion

        #region Protected

        protected ABSMathOperatorImplementation( string key ) : base( key )
        {
        }

        protected abstract decimal Execute( decimal l, decimal r );

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new BSObject( Execute( lD, rD ) );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        #endregion
    }

}

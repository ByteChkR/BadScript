using System;

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public abstract class BSRelationalOperatorImplementation : ABSOperatorImplementation
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

        protected BSRelationalOperatorImplementation( string key ) : base( key )
        {
        }

        protected abstract bool Execute( decimal l, decimal r );

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new BSObject( ( decimal ) ( Execute( lD, rD ) ? 1 : 0 ) );
                }

                throw new Exception( "Invalid Operator Usage" );
            }

            throw new Exception( "Invalid Operator Usage" );
        }

        #endregion

    }

}

using System;

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public abstract class ABSLogicOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public abstract bool Execute( bool l, bool r );

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            return lVal.TryConvertBool( out bool _ ) && rVal.TryConvertBool( out bool _ );
        }

        #endregion

        #region Protected

        protected ABSLogicOperatorImplementation( string key ) : base( key )
        {
        }

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( lVal.TryConvertBool( out bool lD ) )
            {
                if ( rVal.TryConvertBool( out bool rD ) )
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

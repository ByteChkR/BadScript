using BadScript.Exceptions;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.OperatorImplementations.Implementations.Math
{

    public class BSAddOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public BSAddOperatorImplementation() : base( "+" )
        {
        }

        public static ABSObject Add( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( lVal.TryConvertDecimal( out decimal lD ) )
            {
                if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new BSObject( lD + rD );
                }

                if ( rVal.TryConvertString( out string rS ) )
                {
                    return new BSObject( lD + rS );
                }

                throw new BSInvalidOperationException( lVal.Position, "+(Add)", lVal, rVal );
            }

            if ( lVal.TryConvertString( out string lS ) )
            {
                if ( rVal.TryConvertString( out string rStr ) )
                {
                    return new BSObject( lS + rStr );
                }
                else if ( rVal.TryConvertDecimal( out decimal rD ) )
                {
                    return new BSObject( lS + rD );
                }
                else if ( rVal.TryConvertBool( out bool rB ) )
                {
                    return new BSObject( lS + rB );
                }
            }

            if ( lVal.TryConvertBool( out bool lB ) )
            {
                if ( rVal.TryConvertString( out string rStr ) )
                {
                    return new BSObject( lB + rStr );
                }
            }

            throw new BSInvalidOperationException( lVal.Position, "+(Add)", lVal, rVal );
        }

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( lVal.TryConvertString( out string _ ) &&
                 ( rVal.TryConvertBool( out bool _ ) ||
                   rVal.TryConvertDecimal( out decimal _ ) ||
                   rVal.TryConvertString( out string _ ) ) )
            {
                return true;
            }

            if ( lVal.TryConvertDecimal( out decimal _ ) &&
                 ( rVal.TryConvertDecimal( out decimal _ ) || rVal.TryConvertString( out string v ) ) )
            {
                return true;
            }

            if ( lVal.TryConvertBool( out bool _ ) &&
                 ( rVal.TryConvertBool( out bool _ ) || rVal.TryConvertString( out string _ ) ) )
            {
                return true;
            }

            throw new BSRuntimeException( rVal.Position, $"Can not convert objects '{lVal}', '{rVal}'" );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            return Add( arg );
        }

        #endregion

    }

}

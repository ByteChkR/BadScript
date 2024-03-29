﻿using BadScript.Exceptions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations.Math.Self
{

    public abstract class ABSSelfMathOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( !lVal.TryConvertDecimal( out decimal _ ) )
            {
                throw new BSRuntimeException( $"Can not convert object '{lVal}' to a decimal" );
            }

            if ( !rVal.TryConvertDecimal( out decimal _ ) )
            {
                throw new BSRuntimeException( $"Can not convert object '{rVal}' to a decimal" );
            }

            return true;
        }

        #endregion

        #region Protected

        protected ABSSelfMathOperatorImplementation( string key ) : base( key )
        {
        }

        protected abstract decimal Execute( decimal l, decimal r );

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject left = arg[0];

            if ( left is ABSReference lRef )
            {
                ABSObject lVal = left.ResolveReference();
                ABSObject rVal = arg[1].ResolveReference();
                decimal lD = lVal.ConvertDecimal();
                decimal rD = rVal.ConvertDecimal();

                lRef.Assign( new BSObject( Execute( lD, rD ) ) );

                return lRef;
            }
            else
            {
                throw new BSInvalidTypeException(
                                                 left.Position,
                                                 "Expected Assignable Reference",
                                                 left,
                                                 "Reference"
                                                );
            }
        }

        #endregion

    }

}

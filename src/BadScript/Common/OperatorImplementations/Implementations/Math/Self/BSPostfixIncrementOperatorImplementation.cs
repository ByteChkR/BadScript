﻿using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations.Math.Self
{

    public class BSPostfixIncrementOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public BSPostfixIncrementOperatorImplementation() : base( "++_Post" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            ABSObject lVal = args[0];

            if ( !lVal.TryConvertDecimal( out decimal _ ) )
            {
                throw new BSRuntimeException( $"Can not convert object '{lVal}' to a decimal" );
            }

            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject left = args[0];

            if ( left is ABSReference lRef )
            {
                ABSObject oldVal = lRef.ResolveReference();
                lRef.Assign( new BSObject( lRef.ConvertDecimal() + 1 ) );

                return oldVal;
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

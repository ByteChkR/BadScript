using BadScript.Exceptions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations.Math.Self
{

    public class BSPrefixIncrementOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public BSPrefixIncrementOperatorImplementation() : base( "++_Pre" )
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
                lRef.Assign( new BSObject( lRef.ConvertDecimal() + 1 ) );

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

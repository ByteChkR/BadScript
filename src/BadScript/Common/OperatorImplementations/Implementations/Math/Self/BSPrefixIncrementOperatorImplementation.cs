using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations.Math.Self
{

    public class BSPrefixIncrementOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSPrefixIncrementOperatorImplementation() : base( "++_Pre" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args[0].TryConvertDecimal( out decimal _ );
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
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations.Logic.Self
{

    public abstract class ABSSelfLogicOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public abstract bool Execute( bool l, bool r );

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            if ( !lVal.TryConvertBool( out bool _ ) )
            {
                throw new BSRuntimeException( $"Can not convert object '{lVal}' to a boolean" );
            }

            if ( !rVal.TryConvertBool( out bool _ ) )
            {
                throw new BSRuntimeException( $"Can not convert object '{rVal}' to a boolean" );
            }

            return true;
        }

        #endregion

        #region Protected

        protected ABSSelfLogicOperatorImplementation( string key ) : base( key )
        {
        }

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject left = arg[0];

            if ( left is ABSReference lRef )
            {
                ABSObject right = arg[1].ResolveReference();
                bool lD = left.ConvertBool();
                bool rD = right.ConvertBool();

                lRef.Assign( Execute( lD, rD ) ? BSObject.True : BSObject.False );

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

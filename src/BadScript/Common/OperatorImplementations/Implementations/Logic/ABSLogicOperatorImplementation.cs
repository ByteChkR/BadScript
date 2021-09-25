using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations.Logic
{

    public abstract class ABSLogicOperatorImplementation : ABSOperatorImplementation
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

        protected ABSLogicOperatorImplementation( string key ) : base( key )
        {
        }

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject lVal = arg[0].ResolveReference();
            ABSObject rVal = arg[1].ResolveReference();
            bool lD = lVal.ConvertBool();
            bool rD = rVal.ConvertBool();

            return Execute( lD, rD ) ? BSObject.True : BSObject.False;
        }

        #endregion

    }

}

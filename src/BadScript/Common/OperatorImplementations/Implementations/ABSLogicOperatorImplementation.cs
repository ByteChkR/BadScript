using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

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
            ABSObject lVal = arg[0].ResolveReference();
            ABSObject rVal = arg[1].ResolveReference();
            bool lD = lVal.ConvertBool();
            bool rD = rVal.ConvertBool();

            return new BSObject( ( decimal ) ( Execute( lD, rD ) ? 1 : 0 ) );
        }

        #endregion
    }

}

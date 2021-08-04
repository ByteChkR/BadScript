using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations.Math
{

    public abstract class ABSMathOperatorImplementation : ABSOperatorImplementation
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

        protected ABSMathOperatorImplementation( string key ) : base( key )
        {
        }

        protected abstract decimal Execute( decimal l, decimal r );

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject lVal = arg[0].ResolveReference();
            ABSObject rVal = arg[1].ResolveReference();
            decimal lD = lVal.ConvertDecimal();
            decimal rD = rVal.ConvertDecimal();

            return new BSObject( Execute( lD, rD ) );

        }

        #endregion
    }

}

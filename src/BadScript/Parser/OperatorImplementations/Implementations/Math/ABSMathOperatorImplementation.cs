using BadScript.Exceptions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations.Math
{

    public abstract class ABSMathOperatorImplementation : ABSOperatorImplementation
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

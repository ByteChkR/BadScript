using BadScript.Common.Types;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSNullTestOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSNullTestOperatorImplementation() : base( "??" )
        {
        }

        public static ABSObject NullTest( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];
            ABSObject rVal = arg[1];

            return lVal.IsNull ? rVal : lVal;
        }

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            return NullTest( arg );
        }

        #endregion
    }

}

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSNotOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSNotOperatorImplementation() : base( "!" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] arg )
        {
            ABSObject lVal = arg[0];

            return lVal.TryConvertBool( out bool _ );
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            ABSObject lVal = arg[0].ResolveReference();
            bool lD = lVal.ConvertBool();

            return lD ? BSObject.One : BSObject.Zero;

        }

        #endregion
    }

}

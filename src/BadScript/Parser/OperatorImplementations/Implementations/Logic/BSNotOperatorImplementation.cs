using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations.Logic
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

            return lD ? BSObject.False : BSObject.True;
        }

        #endregion

    }

}

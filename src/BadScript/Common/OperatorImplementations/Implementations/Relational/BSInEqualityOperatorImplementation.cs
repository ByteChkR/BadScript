using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.OperatorImplementations.Implementations.Relational
{

    public class BSInEqualityOperatorImplementation : ABSOperatorImplementation
    {
        #region Public

        public BSInEqualityOperatorImplementation() : base( "!=" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] arg )
        {
            return arg[0].ResolveReference().Equals( arg[1].ResolveReference() ) ? BSObject.Zero : BSObject.One;
        }

        #endregion
    }

}

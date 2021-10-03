using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations.Relational
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
            return arg[0].ResolveReference().Equals( arg[1].ResolveReference() ) ? BSObject.False : BSObject.True;
        }

        #endregion

    }

}

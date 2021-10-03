using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.OperatorImplementations.Implementations.Relational
{

    public class BSEqualityOperatorImplementation : ABSOperatorImplementation
    {

        #region Public

        public BSEqualityOperatorImplementation() : base( "==" )
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
            return arg[0].ResolveReference().Equals( arg[1].ResolveReference() ) ? BSObject.True : BSObject.False;
        }

        #endregion

    }

}

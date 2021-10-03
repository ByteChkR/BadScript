using BadScript.Types;

namespace BadScript.Parser.OperatorImplementations.Implementations
{

    public class BSPropertyExpressionImplementation : ABSOperatorImplementation
    {

        #region Public

        public BSPropertyExpressionImplementation() : base( "." )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        #endregion

        #region Protected

        protected override ABSObject Execute( ABSObject[] args )
        {
            return args[0].GetProperty( args[1].ConvertString() );
        }

        #endregion

    }

}

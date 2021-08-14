using BadScript.Common.Types;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSPropertyExpressionImplementation:ABSOperatorImplementation
    {
        public BSPropertyExpressionImplementation( ) : base( "." )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        protected override ABSObject Execute( ABSObject[] args )
        {

            return args[0].GetProperty(args[1].ConvertString());
        }
    }

}
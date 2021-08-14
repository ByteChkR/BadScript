using System.Linq;
using BadScript.Common.Types;

namespace BadScript.Common.OperatorImplementations.Implementations
{

    public class BSInvocationExpressionOperatorImplementation:ABSOperatorImplementation
    {
        public BSInvocationExpressionOperatorImplementation() : base( "()" )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return true;
        }

        protected override ABSObject Execute( ABSObject[] args )
        {
            ABSObject obj = args[0];
            ABSObject[] parameters = args.Skip( 1 ).ToArray();

            return obj.Invoke( parameters );
        }
    }

}
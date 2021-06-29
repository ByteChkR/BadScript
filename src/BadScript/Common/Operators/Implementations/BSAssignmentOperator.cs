using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Binary;

namespace BadScript.Common.Operators.Implementations
{

    public class BSAssignmentOperator : BSOperator
    {
        public override string OperatorKey => "=";

        public override int Preceedence => 15;

        #region Public

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSAssignExpression( left, parser.ParseExpression( Preceedence - 1 ) );
        }

        #endregion
    }

}

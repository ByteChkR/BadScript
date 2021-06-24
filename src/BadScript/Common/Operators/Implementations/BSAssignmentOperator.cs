using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Binary;

namespace BadScript.Common.Operators.Implementations
{

    public class BSAssignmentOperator : BSOperator
    {
        public override string OperatorKey => "=";

        #region Public

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSAssignExpression( left, parser.ParseExpression( 0 ) );
        }

        #endregion
    }

}

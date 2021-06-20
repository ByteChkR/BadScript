using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Binary;

namespace BadScript.Parser.Operators
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

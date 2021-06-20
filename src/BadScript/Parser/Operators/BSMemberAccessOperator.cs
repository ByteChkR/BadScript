using BadScript.Parser.Expressions;

namespace BadScript.Parser.Operators
{

    public class BSMemberAccessOperator : BSOperator
    {

        public override string OperatorKey => ".";

        #region Public

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return parser.ParseWord( left );
        }

        #endregion

    }

}

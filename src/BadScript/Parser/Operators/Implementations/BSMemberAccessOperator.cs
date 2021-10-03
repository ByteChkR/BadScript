using BadScript.Parser.Expressions;

namespace BadScript.Parser.Operators.Implementations
{

    public class BSMemberAccessOperator : BSOperator
    {

        public override string OperatorKey => ".";

        public override int Preceedence => 2;

        #region Public

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return parser.ParseWord( left );
        }

        #endregion

    }

}

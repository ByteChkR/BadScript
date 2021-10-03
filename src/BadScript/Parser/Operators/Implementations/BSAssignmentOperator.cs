using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Binary;

namespace BadScript.Parser.Operators.Implementations
{

    public class BSAssignmentOperator : BSOperator
    {

        public override string OperatorKey => "=";

        public override int Preceedence => 15;

        #region Public

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSAssignExpression(
                                          parser.CreateSourcePosition(),
                                          left,
                                          parser.ParseExpression( Preceedence - 1 )
                                         );
        }

        #endregion

    }

}

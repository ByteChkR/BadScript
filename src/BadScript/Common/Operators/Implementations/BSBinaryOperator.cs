using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSBinaryOperator : BSOperator
    {
        private int m_Start;

        private BSFunction m_OperatorImplementation;

        public override string OperatorKey { get; }

        #region Public

        public BSBinaryOperator( int start, string key, BSFunction func )
        {
            OperatorKey = key;
            m_OperatorImplementation = func;
            m_Start = start;
        }

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSInvocationExpression(
                new BSProxyExpression( m_OperatorImplementation ),
                new[] { left, parser.ParseExpression( m_Start ) }
            );
        }

        #endregion
    }

}

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSBinaryOperator : BSOperator
    {
        private BSFunction m_OperatorImplementation;

        public override string OperatorKey { get; }

        public override int Preceedence { get; }

        #region Public

        public BSBinaryOperator( int start, string key, BSFunction func )
        {
            OperatorKey = key;
            m_OperatorImplementation = func;
            Preceedence = start;
        }

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSInvocationExpression(
                new BSProxyExpression( m_OperatorImplementation ),
                new[] { left, parser.ParseExpression( Preceedence - 1 ) }
            );
        }

        #endregion
    }

}

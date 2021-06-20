using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Value;
using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Operators
{

    public class BSRuntimeBinaryOperator : BSOperator
    {

        private class BSProxyExpression : BSExpression
        {

            private BSRuntimeObject m_Object;

            #region Public

            public BSProxyExpression( BSRuntimeObject obj )
            {
                m_Object = obj;
            }

            public override BSRuntimeObject Execute( BSEngineScope scope )
            {
                return m_Object;
            }

            #endregion

        }

        private int m_Start;

        private BSRuntimeFunction m_OperatorImplementation;

        public override string OperatorKey { get; }

        #region Public

        public BSRuntimeBinaryOperator( int start, string key, BSRuntimeFunction func )
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

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Value;
using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Operators
{

    public class BSRuntimeUnaryOperator : BSOperator
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

        private BSRuntimeFunction m_OperatorImplementation;

        public override string OperatorKey { get; }

        #region Public

        public BSRuntimeUnaryOperator( string key, BSRuntimeFunction func )
        {
            OperatorKey = key;
            m_OperatorImplementation = func;
        }

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSInvocationExpression( new BSProxyExpression( m_OperatorImplementation ), new[] { left } );
        }

        #endregion

    }

}

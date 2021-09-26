using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSUnaryOperator : BSOperator
    {

        private BSFunction m_OperatorImplementation;
        private BSUnaryOperatorMetaData m_Meta;

        public override string OperatorKey { get; }

        public override int Preceedence => 0;

        #region Public

        public BSUnaryOperator( string key, string implKey, string sig, int argC )
        {
            OperatorKey = key;
            m_Meta = new BSUnaryOperatorMetaData( key, implKey, sig, argC );
            m_OperatorImplementation = m_Meta.MakeFunction();
        }

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSInvocationExpression(
                                              parser.CreateSourcePosition(),
                                              new BSProxyExpression(
                                                                    parser.CreateSourcePosition(),
                                                                    m_OperatorImplementation,
                                                                    m_Meta
                                                                   ),
                                              new[] { left }
                                             );
        }

        #endregion

    }

}

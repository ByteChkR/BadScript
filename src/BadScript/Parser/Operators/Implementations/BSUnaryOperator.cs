using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Types;

namespace BadScript.Parser.Operators.Implementations
{

    public class BSUnaryOperator : BSOperator
    {

        private readonly BSFunction m_OperatorImplementation;
        private readonly BSUnaryOperatorMetaData m_Meta;

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
                                              left.Position,
                                              new BSProxyExpression(
                                                                    left.Position,
                                                                    m_OperatorImplementation,
                                                                    m_Meta
                                                                   ),
                                              new[] { left }
                                             );
        }

        #endregion

    }

}

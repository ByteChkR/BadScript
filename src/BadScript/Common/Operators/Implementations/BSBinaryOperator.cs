using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSBinaryOperator : BSOperator
    {
        private BSFunction m_OperatorImplementation;
        private BSBinaryOperatorMetaData m_Meta;

        public override string OperatorKey { get; }

        public override int Preceedence { get; }

        #region Public

        public BSBinaryOperator( int start, string key, string sig, int argC )
        {
            OperatorKey = key;
            m_Meta = new BSBinaryOperatorMetaData( key, sig, argC );

            m_OperatorImplementation = new BSFunction(
                $"function {key}({sig})",
                objects => BSOperatorImplementationResolver.
                           ResolveImplementation( key, objects ).
                           ExecuteOperator( objects ),
                argC
            );

            Preceedence = start;
        }

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSInvocationExpression(
                parser.CreateSourcePosition(),
                new BSProxyExpression( parser.CreateSourcePosition(), m_OperatorImplementation, m_Meta ),
                new[] { left, parser.ParseExpression( Preceedence - 1 ) }
            );
        }

        #endregion
    }

}

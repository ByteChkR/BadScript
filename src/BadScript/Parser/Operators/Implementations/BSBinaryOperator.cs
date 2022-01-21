using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Parser.OperatorImplementations;
using BadScript.Types;

namespace BadScript.Parser.Operators.Implementations
{

    public class BSBinaryOperator : BSOperator
    {

        private readonly BSFunction m_OperatorImplementation;
        private readonly BSBinaryOperatorMetaData m_Meta;

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
            SourcePosition pos = parser.CreateSourcePosition( parser.GetPosition() );

            return new BSInvocationExpression(
                                              pos,
                                              new BSProxyExpression(
                                                                    pos,
                                                                    m_OperatorImplementation,
                                                                    m_Meta
                                                                   ),
                                              new[] { left, parser.ParseExpression( Preceedence - 1 ) }
                                             );
        }

        #endregion

    }

}

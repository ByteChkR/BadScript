﻿using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Types;

namespace BadScript.Common.Operators.Implementations
{

    public class BSUnaryOperator : BSOperator
    {
        private BSFunction m_OperatorImplementation;

        public override string OperatorKey { get; }

        public override int Preceedence => 0;

        #region Public

        public BSUnaryOperator( string key, BSFunction func )
        {
            OperatorKey = key;
            m_OperatorImplementation = func;
        }

        public override BSExpression Parse( BSExpression left, BSParser parser )
        {
            return new BSInvocationExpression(
                parser.CreateSourcePosition(),
                new BSProxyExpression( parser.CreateSourcePosition(), m_OperatorImplementation ),
                new[] { left } );
        }

        #endregion
    }

}

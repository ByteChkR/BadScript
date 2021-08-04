﻿using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSTableExpression : BSExpression
    {
        private readonly Dictionary < string, BSExpression > m_InitExpressions;

        public override bool IsConstant => m_InitExpressions.All( x => x.Value.IsConstant );

        #region Public

        public BSTableExpression( SourcePosition pos, Dictionary < string, BSExpression > initExprs = null ) : base(
            pos )
        {
            m_InitExpressions = initExprs ?? new Dictionary < string, BSExpression >();
        }

        public override ABSObject Execute( BSScope scope )
        {
            Dictionary < ABSObject, ABSObject > objs = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, BSExpression > initExpression in m_InitExpressions )
            {
                objs[new BSObject( initExpression.Key )] = initExpression.Value.Execute( scope );
            }

            return new BSTable( m_Position, objs );
        }

        #endregion
    }

}

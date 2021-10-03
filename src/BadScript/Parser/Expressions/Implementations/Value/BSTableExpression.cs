using System.Collections.Generic;
using System.Linq;

using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Value
{

    public class BSTableExpression : BSExpression
    {

        public readonly Dictionary < string, BSExpression > InitExpressions;

        public override bool IsConstant => InitExpressions.All( x => x.Value.IsConstant );

        #region Public

        public BSTableExpression( SourcePosition pos, Dictionary < string, BSExpression > initExprs = null ) : base(
             pos
            )
        {
            InitExpressions = initExprs ?? new Dictionary < string, BSExpression >();
        }

        public override ABSObject Execute( BSScope scope )
        {
            Dictionary < ABSObject, ABSObject > objs = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, BSExpression > initExpression in InitExpressions )
            {
                objs[new BSObject( initExpression.Key )] = initExpression.Value.Execute( scope );
            }

            return new BSTable( m_Position, objs );
        }

        #endregion

    }

}

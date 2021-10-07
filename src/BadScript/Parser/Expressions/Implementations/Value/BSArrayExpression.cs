using System;
using System.Linq;

using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Value
{

    public class BSArrayExpression : BSExpression
    {

        public readonly BSExpression[] InitExpressions;

        public override bool IsConstant => InitExpressions.All( x => x.IsConstant );

        #region Public

        public BSArrayExpression( SourcePosition pos, BSExpression[] initExprs = null ) : base( pos )
        {
            InitExpressions = initExprs ?? Array.Empty < BSExpression >();
        }

        public override ABSObject Execute( BSScope scope )
        {
            return new BSArray( InitExpressions.Select( x => x.Execute( scope ) ) );
        }

        #endregion

    }

}

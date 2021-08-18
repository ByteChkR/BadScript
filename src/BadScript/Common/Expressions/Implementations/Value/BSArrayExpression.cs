using System.Linq;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSArrayExpression : BSExpression
    {
        public readonly BSExpression[] InitExpressions;

        public override bool IsConstant => InitExpressions.All( x => x.IsConstant );

        #region Public

        public BSArrayExpression( SourcePosition pos, BSExpression[] initExprs = null ) : base( pos )
        {
            InitExpressions = initExprs ?? new BSExpression[0];
        }

        public override ABSObject Execute( BSScope scope )
        {
            return new BSArray( InitExpressions.Select( x => x.Execute( scope ) ) );
        }

        #endregion
    }

}

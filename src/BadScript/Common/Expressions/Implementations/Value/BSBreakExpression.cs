using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSBreakExpression : BSExpression
    {
        public override bool IsConstant => false;

        #region Public

        public BSBreakExpression( SourcePosition srcPos ) : base( srcPos )
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            scope.SetFlag( BSScopeFlags.Break );

            return BSObject.Null;
        }

        #endregion
    }

}

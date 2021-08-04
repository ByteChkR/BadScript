using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSContinueExpression : BSExpression
    {
        public override bool IsConstant => false;

        #region Public

        public BSContinueExpression( SourcePosition srcPos ) : base( srcPos )
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            scope.SetFlag( BSScopeFlags.Continue );

            return BSObject.Null;
        }

        #endregion
    }

}

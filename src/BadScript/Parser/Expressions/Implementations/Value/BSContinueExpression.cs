using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Value
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

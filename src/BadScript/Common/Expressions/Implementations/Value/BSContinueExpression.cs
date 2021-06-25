using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSContinueExpression : BSExpression
    {
        #region Public

        public BSContinueExpression()
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            scope.SetFlag( BSScopeFlags.Continue );

            return new BSObject( null );
        }

        #endregion
    }

}

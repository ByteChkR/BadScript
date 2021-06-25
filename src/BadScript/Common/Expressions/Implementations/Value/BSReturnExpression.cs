using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSReturnExpression : BSExpression
    {
        public BSExpression Left { get; private set; }

        #region Public

        public BSReturnExpression(BSExpression left)
        {
            Left = left;
        }

        public override ABSObject Execute(BSScope scope)
        {
            ABSObject o = Left.Execute(scope).ResolveReference();

            scope.SetFlag(BSScopeFlags.Return, o);

            return o;
        }

        #endregion
    }

    public class BSBreakExpression : BSExpression
    {

        #region Public

        public BSBreakExpression()
        {
        }

        public override ABSObject Execute(BSScope scope)
        {
            scope.SetFlag(BSScopeFlags.Break);

            return new BSObject(null);
        }

        #endregion
    }

    public class BSContinueExpression : BSExpression
    {

        #region Public

        public BSContinueExpression()
        {
        }

        public override ABSObject Execute(BSScope scope)
        {
            scope.SetFlag(BSScopeFlags.Continue);

            return new BSObject(null);
        }

        #endregion
    }

}

using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Access
{

    public class BSNullCheckPropertyExpression : BSExpression
    {
        public readonly string Right;

        public BSExpression Left { get; private set; }

        #region Public

        public BSNullCheckPropertyExpression(BSExpression left, string varname)
        {
            Left = left;
            Right = varname;
        }

        public override ABSObject Execute(BSScope scope)
        {
            if (Left != null)
            {
                ABSObject l = Left.Execute( scope );

                if (!l.HasProperty(Right))
                    return new BSObject( null );
                return Left.Execute(scope).GetProperty(Right);
            }

            return scope.ResolveName(Right);
        }

        #endregion
    }

}
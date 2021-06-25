using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Expressions.Implementations.Access
{

    public class BSPropertyExpression : BSExpression
    {
        public readonly string Right;

        public BSExpression Left { get; private set; }

        #region Public

        public BSPropertyExpression( BSExpression left, string varname )
        {
            Left = left;
            Right = varname;
        }

        public override ABSObject Execute( BSScope scope )
        {
            if ( Left != null )
            {
                return Left.Execute( scope ).GetProperty( Right );
            }

            return scope.ResolveName( Right );
        }

        #endregion
    }

}

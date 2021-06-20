using BadScript.Runtime;

namespace BadScript.Parser.Expressions.Value
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

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            if ( Left != null )
            {
                return Left.Execute( scope ).GetOrAddProperty( Right );
            }

            return scope.ResolveName( Right );
        }

        #endregion

    }

}

using BadScript.Common.OperatorImplementations;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Access
{

    public class BSPropertyExpression : BSExpression
    {
        public readonly string Right;

        public override bool IsConstant => false;

        public BSExpression Left { get; private set; }

        #region Public

        public BSPropertyExpression( SourcePosition srcPos, BSExpression left, string varname ) : base( srcPos )
        {
            Left = left;
            Right = varname;
        }

        public override ABSObject Execute( BSScope scope )
        {
            if ( Left != null )
            {
                ABSObject l = Left.Execute( scope );

                ABSOperatorImplementation impl = BSOperatorImplementationResolver.ResolveImplementation(
                    ".",
                    new[] { l, new BSObject( Right ) },
                    true );

                return impl.ExecuteOperator( new[] { l, new BSObject( Right ) } );
            }

            return scope.ResolveName( Right );
        }

        #endregion
    }

}

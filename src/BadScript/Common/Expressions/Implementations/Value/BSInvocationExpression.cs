using System.Linq;
using BadScript.Common.Expressions.Implementations.Unary;
using BadScript.Common.OperatorImplementations;
using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSInvocationExpression : BSUnaryExpression
    {
        public BSExpression[] Parameters;

        public override bool IsConstant => Left.IsConstant && Parameters.All( x => x.IsConstant );

        #region Public

        public BSInvocationExpression( SourcePosition srcPos, BSExpression left, BSExpression[] args ) : base(
            srcPos,
            left )
        {
            Parameters = args;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject obj = Left.Execute( scope );
            ABSObject[] args = new[] { obj }.Concat( Parameters.Select( x => x.Execute( scope ) ) ).ToArray();
            ABSOperatorImplementation impl = BSOperatorImplementationResolver.ResolveImplementation( "()", args, true );

            return impl.ExecuteOperator( args );
        }

        #endregion
    }

}

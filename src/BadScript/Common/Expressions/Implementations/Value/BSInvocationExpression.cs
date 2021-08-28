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
            ABSObject[] args = new ABSObject[Parameters.Length + 1];
            args[0] = obj;

            for ( int i = 1; i < args.Length; i++ )
            {
                args[i] = Parameters[i - 1].Execute( scope );
            }

            ABSOperatorImplementation impl = BSOperatorImplementationResolver.ResolveImplementation( "()", args, true );

            return impl.ExecuteOperator( args );
        }

        #endregion
    }

}

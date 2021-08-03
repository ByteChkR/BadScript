using System.Linq;
using BadScript.Common.Expressions.Implementations.Unary;
using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSInvocationExpression : BSUnaryExpression
    {
        private BSExpression[] Parameters;

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

            return obj.Invoke(
                Parameters.Select( x => x.Execute( scope ) ).
                           ToArray()
            );
        }

        #endregion
    }

}

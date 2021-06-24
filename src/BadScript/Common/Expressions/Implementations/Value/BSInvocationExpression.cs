using System.Linq;
using BadScript.Common.Expressions.Implementations.Unary;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSInvocationExpression : BSUnaryExpression
    {
        private BSExpression[] Parameters;

        #region Public

        public BSInvocationExpression( BSExpression left, BSExpression[] args ) : base( left )
        {
            Parameters = args;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject obj = Left.Execute( scope );

            return obj.Invoke(
                Parameters.Select( x => x.Execute( scope ) ).
                           Select( x => x.ResolveReference() ).
                           ToArray()
            );
        }

        #endregion
    }

}

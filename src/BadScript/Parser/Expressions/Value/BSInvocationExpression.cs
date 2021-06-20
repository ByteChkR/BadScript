using System.Linq;

using BadScript.Parser.Expressions.Unary;
using BadScript.Runtime;

namespace BadScript.Parser.Expressions.Value
{

    public class BSInvocationExpression : BSUnaryExpression
    {

        private BSExpression[] Parameters;

        #region Public

        public BSInvocationExpression( BSExpression left, BSExpression[] args ) : base( left )
        {
            Parameters = args;
        }

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            BSRuntimeObject obj = Left.Execute( scope );

            return obj.Invoke(
                              Parameters.Select( x => x.Execute( scope ) ).
                                         Select( x => ( x as BSRuntimeReference )?.Get() ?? x ).
                                         ToArray()
                             );
        }

        #endregion

    }

}

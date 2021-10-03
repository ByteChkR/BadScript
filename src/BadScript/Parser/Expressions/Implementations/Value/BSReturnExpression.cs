using BadScript.Parser.Expressions.Implementations.Unary;
using BadScript.Scopes;
using BadScript.Types;

namespace BadScript.Parser.Expressions.Implementations.Value
{

    public class BSReturnExpression : BSUnaryExpression
    {

        public override bool IsConstant => false;

        #region Public

        public BSReturnExpression( SourcePosition srcPos, BSExpression left ) : base( srcPos, left )
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject o = Left.Execute( scope );

            scope.SetFlag( BSScopeFlags.Return, o );

            return o;
        }

        #endregion

    }

}

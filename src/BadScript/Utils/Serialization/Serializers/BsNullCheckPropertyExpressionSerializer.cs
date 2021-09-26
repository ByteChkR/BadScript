using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Utils.Serialization.Serializers
{

    public class BsNullCheckPropertyExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.NullCheckPropertyAccessExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSNullCheckPropertyExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression e = s.DeserializeExpression();

            return new BSNullCheckPropertyExpression( SourcePosition.Unknown, e, s.DeserializeString() );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSNullCheckPropertyExpression expr = ( BSNullCheckPropertyExpression )e;

            ret.SerializeOpCode( BSCompiledExpressionCode.NullCheckPropertyAccessExpr );
            ret.SerializeExpression( expr.Left );
            ret.SerializeString( expr.Right );
        }

        #endregion

    }

}

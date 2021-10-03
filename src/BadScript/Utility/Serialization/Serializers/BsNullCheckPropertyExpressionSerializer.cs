using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Utility.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            BSExpression e = s.DeserializeExpression(context);

            return new BSNullCheckPropertyExpression( SourcePosition.Unknown, e, s.DeserializeString(context) );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSNullCheckPropertyExpression expr = ( BSNullCheckPropertyExpression )e;

            ret.SerializeOpCode( BSCompiledExpressionCode.NullCheckPropertyAccessExpr );
            ret.SerializeExpression( expr.Left , context);
            ret.SerializeString( expr.Right, context );
        }

        #endregion

    }

}

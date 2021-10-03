using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsInvocationExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.InvocationExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSInvocationExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            BSExpression l = s.DeserializeExpression(context);

            return new BSInvocationExpression( SourcePosition.Unknown, l, s.DeserializeBlock(context) );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSInvocationExpression expr = ( BSInvocationExpression )e;

            ret.SerializeOpCode( BSCompiledExpressionCode.InvocationExpr );
            ret.SerializeExpression( expr.Left, context);
            ret.SerializeBlock( expr.Parameters, context);
        }

        #endregion

    }

}

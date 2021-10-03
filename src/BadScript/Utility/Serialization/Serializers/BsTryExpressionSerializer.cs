using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsTryExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.TryExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSTryExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            BSExpression[] tryBlock = s.DeserializeBlock(context);
            string cVar = s.DeserializeString(context);
            BSExpression[] catchBlock = s.DeserializeBlock(context);

            return new BSTryExpression( SourcePosition.Unknown, tryBlock, catchBlock, cVar );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSTryExpression expr = ( BSTryExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.TryExpr );
            ret.SerializeBlock( expr.TryBlock, context);
            ret.SerializeString( expr.CapturedVar, context);
            ret.SerializeBlock( expr.CatchBlock, context);
        }

        #endregion

    }

}

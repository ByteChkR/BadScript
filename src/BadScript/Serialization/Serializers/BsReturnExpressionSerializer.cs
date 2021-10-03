using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Value;

namespace BadScript.Serialization.Serializers
{

    public class BsReturnExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ReturnExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSReturnExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            return new BSReturnExpression( SourcePosition.Unknown, s.DeserializeExpression( context ) );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            BSReturnExpression expr = ( BSReturnExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.ReturnExpr );
            ret.SerializeExpression( expr.Left, context );
        }

        #endregion

    }

}

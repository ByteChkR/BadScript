using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            return new BSReturnExpression( SourcePosition.Unknown, s.DeserializeExpression() );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSReturnExpression expr = ( BSReturnExpression ) e;
            ret.SerializeOpCode( BSCompiledExpressionCode.ReturnExpr );
            ret.SerializeExpression( expr.Left );

        }

        #endregion
    }

}

using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Serialization.Serializers
{

    public class BsWhileExpressionSerializer : BSExpressionSerializer
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.WhileExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSWhileExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression cond = s.DeserializeExpression();

            return new BSWhileExpression( SourcePosition.Unknown, cond, s.DeserializeBlock() );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSWhileExpression expr = ( BSWhileExpression ) e;
            ret.SerializeOpCode( BSCompiledExpressionCode.WhileExpr );
            ret.SerializeExpression( expr.Condition );
            ret.SerializeBlock( expr.Block );

        }

        #endregion
    }

}

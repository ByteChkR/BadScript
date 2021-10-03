using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block;

namespace BadScript.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            BSExpression cond = s.DeserializeExpression( context );

            return new BSWhileExpression( SourcePosition.Unknown, cond, s.DeserializeBlock( context ) );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            BSWhileExpression expr = ( BSWhileExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.WhileExpr );
            ret.SerializeExpression( expr.Condition, context );
            ret.SerializeBlock( expr.Block, context );
        }

        #endregion

    }

}

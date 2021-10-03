using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;

namespace BadScript.Serialization.Serializers
{

    public class BsForExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ForExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSForExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            BSExpression cDef = s.DeserializeExpression( context );
            BSExpression cCond = s.DeserializeExpression( context );
            BSExpression cInc = s.DeserializeExpression( context );
            BSExpression[] block = s.DeserializeBlock( context );

            return new BSForExpression( SourcePosition.Unknown, cDef, cCond, cInc, block );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            BSForExpression expr = ( BSForExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.ForExpr );
            ret.SerializeExpression( expr.CounterDefinition, context );
            ret.SerializeExpression( expr.CounterCondition, context );
            ret.SerializeExpression( expr.CounterIncrement, context );
            ret.SerializeBlock( expr.Block, context );
        }

        #endregion

    }

}

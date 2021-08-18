using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSForExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSForExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ForExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression cDef = s.DeserializeExpression();
            BSExpression cCond = s.DeserializeExpression();
            BSExpression cInc = s.DeserializeExpression();
            BSExpression[] block = s.DeserializeBlock();
            return new BSForExpression(SourcePosition.Unknown, cDef, cCond, cInc, block);
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSForExpression expr = (BSForExpression)e;
            List<byte> ret = new List<byte>();
            ret.SerializeOpCode(BSCompiledExpressionCode.ForExpr);
            ret.SerializeExpression(expr.CounterDefinition);
            ret.SerializeExpression(expr.CounterCondition);
            ret.SerializeExpression(expr.CounterIncrement);
            ret.SerializeBlock( expr.Block );

            return ret.ToArray();
        }
    }

}
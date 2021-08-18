using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSIfExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSIfExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.IfExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            Dictionary < BSExpression, BSExpression[] > map = s.DeserializeMap();
            BSExpression[] elseBlock = s.DeserializeBlock();

            return new BSIfExpression( SourcePosition.Unknown, map, elseBlock );
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSIfExpression expr = (BSIfExpression)e;
            List<byte> ret = new List<byte>();
            ret.SerializeOpCode(BSCompiledExpressionCode.IfExpr);
            ret.SerializeMap( expr.ConditionMap );
            ret.SerializeBlock( expr.ElseBlock ?? new BSExpression[0] );
            return ret.ToArray();
        }
    }

}
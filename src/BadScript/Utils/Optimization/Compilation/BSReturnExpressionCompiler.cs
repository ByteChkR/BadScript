using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSReturnExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSReturnExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ReturnExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            return new BSReturnExpression(SourcePosition.Unknown, s.DeserializeExpression());
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSReturnExpression expr = (BSReturnExpression)e;
            List<byte> ret = new List<byte>();
            ret.SerializeOpCode(BSCompiledExpressionCode.ReturnExpr);
            ret.SerializeExpression( expr.Left );

            return ret.ToArray();
        }
    }

}
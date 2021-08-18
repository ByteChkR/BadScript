using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSArrayAccessExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSArrayAccessExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ArrayAccessExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression l = s.DeserializeExpression();

            return new BSArrayAccessExpression( SourcePosition.Unknown, l, s.DeserializeExpression() );
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSArrayAccessExpression expr = (BSArrayAccessExpression)e;
            List<byte> ret = new List<byte>();

            ret.SerializeOpCode(BSCompiledExpressionCode.ArrayAccessExpr);
            ret.SerializeExpression(expr.Left);
            ret.SerializeExpression(expr.Right);

            return ret.ToArray();
        }
    }

}
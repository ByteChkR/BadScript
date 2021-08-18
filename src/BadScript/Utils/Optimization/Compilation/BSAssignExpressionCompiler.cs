using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Binary;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSAssignExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSAssignExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.AssignExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression l = s.DeserializeExpression();

            return new BSAssignExpression( SourcePosition.Unknown, l, s.DeserializeExpression() );
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSAssignExpression expr = (BSAssignExpression)e;
            List<byte> ret = new List<byte>();
            ret.SerializeOpCode(BSCompiledExpressionCode.AssignExpr);
            ret.SerializeExpression(expr.Left);
            ret.SerializeExpression(expr.Right);

            return ret.ToArray();
        }
    }

}
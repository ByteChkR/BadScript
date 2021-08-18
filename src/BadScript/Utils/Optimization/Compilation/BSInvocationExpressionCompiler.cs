using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSInvocationExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSInvocationExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.InvocationExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression l = s.DeserializeExpression();

            return new BSInvocationExpression( SourcePosition.Unknown, l, s.DeserializeBlock() );
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSInvocationExpression expr = (BSInvocationExpression)e;
            List<byte> ret = new List<byte>();

            ret.SerializeOpCode(BSCompiledExpressionCode.InvocationExpr);
            ret.SerializeExpression( expr.Left );
            ret.SerializeBlock( expr.Parameters );

            return ret.ToArray();
        }
    }

}
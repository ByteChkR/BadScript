using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSPropertyExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSPropertyExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.PropertyAccessExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            bool hasVal = s.DeserializeBool();
            BSExpression expr = null;
            if ( hasVal )
            {
                expr = s.DeserializeExpression();
            }

            string r = s.DeserializeString();

            return new BSPropertyExpression( SourcePosition.Unknown, expr, r );
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSPropertyExpression expr = (BSPropertyExpression)e;
            List<byte> ret = new List<byte>();

            ret.SerializeOpCode(BSCompiledExpressionCode.PropertyAccessExpr);
            ret.SerializeBool( expr.Left != null );
            if ( expr.Left != null )
                ret.SerializeExpression( expr.Left );
            ret.SerializeString(expr.Right);

            return ret.ToArray();
        }
    }

}
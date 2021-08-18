using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSForeachExpressionCompiler : BSExpressionCompiler
    {
        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSForeachExpression;
        }

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ForEachExpr;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            int vCount = s.DeserializeInt32();
            string[] vars = new string[vCount];

            for ( int i = 0; i < vCount; i++ )
            {
                vars[i] = s.DeserializeString();
            }
            BSExpression enumerator = s.DeserializeExpression();
            BSExpression[] block = s.DeserializeBlock();
            return new BSForeachExpression(SourcePosition.Unknown,vars, enumerator, block);
        }

        public override byte[] Serialize(BSExpression e)
        {
            BSForeachExpression expr = (BSForeachExpression)e;
            List<byte> ret = new List<byte>();
            ret.SerializeOpCode(BSCompiledExpressionCode.ForEachExpr);
            ret.SerializeInt32( expr.Vars.Length );

            foreach ( string exprVar in expr.Vars )
            {
                ret.SerializeString( exprVar );
            }
            ret.SerializeExpression( expr.Enumerator );
            ret.SerializeBlock( expr.Block );

            return ret.ToArray();
        }
    }

}
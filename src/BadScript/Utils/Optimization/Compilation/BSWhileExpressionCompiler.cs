using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSWhileExpressionCompiler : BSExpressionCompiler
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression cond = s.DeserializeExpression();

            return new BSWhileExpression( SourcePosition.Unknown, cond, s.DeserializeBlock() );
        }

        public override byte[] Serialize( BSExpression e )
        {
            BSWhileExpression expr = ( BSWhileExpression ) e;
            List < byte > ret = new List < byte >();
            ret.SerializeOpCode( BSCompiledExpressionCode.WhileExpr );
            ret.SerializeExpression( expr.Condition );
            ret.SerializeBlock( expr.Block );

            return ret.ToArray();
        }

        #endregion
    }

}

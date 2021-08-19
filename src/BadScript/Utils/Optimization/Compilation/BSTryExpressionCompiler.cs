using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSTryExpressionCompiler : BSExpressionCompiler
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.TryExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSTryExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression[] tryBlock = s.DeserializeBlock();
            string cVar = s.DeserializeString();
            BSExpression[] catchBlock = s.DeserializeBlock();

            return new BSTryExpression( SourcePosition.Unknown, tryBlock, catchBlock, cVar );
        }

        public override byte[] Serialize( BSExpression e )
        {
            BSTryExpression expr = ( BSTryExpression ) e;
            List < byte > ret = new List < byte >();
            ret.SerializeOpCode( BSCompiledExpressionCode.TryExpr );
            ret.SerializeBlock( expr.TryBlock );
            ret.SerializeString( expr.CapturedVar );
            ret.SerializeBlock( expr.CatchBlock );

            return ret.ToArray();
        }

        #endregion
    }

}

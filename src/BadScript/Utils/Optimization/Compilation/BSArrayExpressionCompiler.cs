using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSArrayExpressionCompiler : BSExpressionCompiler
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ArrayExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSArrayExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            return new BSArrayExpression( SourcePosition.Unknown, s.DeserializeBlock() );
        }

        public override byte[] Serialize( BSExpression e )
        {
            BSArrayExpression expr = ( BSArrayExpression ) e;
            List < byte > ret = new List < byte >();

            ret.SerializeOpCode( BSCompiledExpressionCode.ArrayExpr );
            ret.SerializeBlock( expr.InitExpressions );

            return ret.ToArray();
        }

        #endregion
    }

}

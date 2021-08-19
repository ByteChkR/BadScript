using System;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSBreakExpressionCompiler : BSExpressionCompiler
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.BreakExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSBreakExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            return new BSBreakExpression( SourcePosition.Unknown );
        }

        public override byte[] Serialize( BSExpression e )
        {
            return BitConverter.GetBytes( ( byte ) BSCompiledExpressionCode.BreakExpr );
        }

        #endregion
    }

}

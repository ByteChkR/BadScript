using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSNullCheckPropertyExpressionCompiler : BSExpressionCompiler
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.NullCheckPropertyAccessExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSNullCheckPropertyExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression e = s.DeserializeExpression();

            return new BSNullCheckPropertyExpression( SourcePosition.Unknown, e, s.DeserializeString() );
        }

        public override byte[] Serialize( BSExpression e )
        {
            BSNullCheckPropertyExpression expr = ( BSNullCheckPropertyExpression ) e;
            List < byte > ret = new List < byte >();

            ret.SerializeOpCode( BSCompiledExpressionCode.NullCheckPropertyAccessExpr );
            ret.SerializeExpression( expr.Left );
            ret.SerializeString( expr.Right );

            return ret.ToArray();
        }

        #endregion
    }

}

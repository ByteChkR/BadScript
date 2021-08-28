using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BsInvocationExpressionSerializer : BSExpressionSerializer
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.InvocationExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSInvocationExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression l = s.DeserializeExpression();

            return new BSInvocationExpression( SourcePosition.Unknown, l, s.DeserializeBlock() );
        }

        public override void Serialize(BSExpression e, Stream ret)
        {
            BSInvocationExpression expr = ( BSInvocationExpression ) e;

            ret.SerializeOpCode( BSCompiledExpressionCode.InvocationExpr );
            ret.SerializeExpression( expr.Left );
            ret.SerializeBlock( expr.Parameters );
            
        }

        #endregion
    }

}

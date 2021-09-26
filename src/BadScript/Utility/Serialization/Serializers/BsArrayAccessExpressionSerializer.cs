using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsArrayAccessExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ArrayAccessExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSArrayAccessExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression l = s.DeserializeExpression();

            return new BSArrayAccessExpression( SourcePosition.Unknown, l, s.DeserializeExpression() );
        }

        public override void Serialize( BSExpression e, Stream s )
        {
            BSArrayAccessExpression expr = ( BSArrayAccessExpression )e;

            s.SerializeOpCode( BSCompiledExpressionCode.ArrayAccessExpr );
            s.SerializeExpression( expr.Left );
            s.SerializeExpression( expr.Right );
        }

        #endregion

    }

}

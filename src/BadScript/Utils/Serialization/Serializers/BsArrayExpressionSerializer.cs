using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Serialization.Serializers
{

    public class BsArrayExpressionSerializer : BSExpressionSerializer
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

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSArrayExpression expr = ( BSArrayExpression )e;

            ret.SerializeOpCode( BSCompiledExpressionCode.ArrayExpr );
            ret.SerializeBlock( expr.InitExpressions );
        }

        #endregion

    }

}

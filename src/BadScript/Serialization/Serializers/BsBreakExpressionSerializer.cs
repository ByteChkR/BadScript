using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Value;

namespace BadScript.Serialization.Serializers
{

    public class BsBreakExpressionSerializer : BSExpressionSerializer
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            return new BSBreakExpression( SourcePosition.Unknown );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            ret.SerializeOpCode( BSCompiledExpressionCode.BreakExpr );
        }

        #endregion

    }

}

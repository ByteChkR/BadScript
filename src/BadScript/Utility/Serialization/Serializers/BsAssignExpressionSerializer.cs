using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Binary;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsAssignExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.AssignExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSAssignExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            BSExpression l = s.DeserializeExpression(context);

            return new BSAssignExpression( SourcePosition.Unknown, l, s.DeserializeExpression(context) );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSAssignExpression expr = ( BSAssignExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.AssignExpr );
            ret.SerializeExpression( expr.Left,context );
            ret.SerializeExpression( expr.Right ,context);
        }

        #endregion

    }

}

using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Access;

namespace BadScript.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            BSExpression l = s.DeserializeExpression( context );

            return new BSArrayAccessExpression( SourcePosition.Unknown, l, s.DeserializeExpression( context ) );
        }

        public override void Serialize( BSExpression e, Stream s, BSSerializerContext context )
        {
            BSArrayAccessExpression expr = ( BSArrayAccessExpression )e;

            s.SerializeOpCode( BSCompiledExpressionCode.ArrayAccessExpr );
            s.SerializeExpression( expr.Left, context );
            s.SerializeExpression( expr.Right, context );
        }

        #endregion

    }

}

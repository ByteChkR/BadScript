using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Access;

namespace BadScript.Serialization.Serializers
{

    public class BsPropertyExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.PropertyAccessExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSPropertyExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            bool hasVal = s.DeserializeBool();
            BSExpression expr = null;

            if ( hasVal )
            {
                expr = s.DeserializeExpression( context );
            }

            string r = s.DeserializeString( context );

            return new BSPropertyExpression( SourcePosition.Unknown, expr, r );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            BSPropertyExpression expr = ( BSPropertyExpression )e;

            ret.SerializeOpCode( BSCompiledExpressionCode.PropertyAccessExpr );
            ret.SerializeBool( expr.Left != null );

            if ( expr.Left != null )
            {
                ret.SerializeExpression( expr.Left, context );
            }

            ret.SerializeString( expr.Right, context );
        }

        #endregion

    }

}

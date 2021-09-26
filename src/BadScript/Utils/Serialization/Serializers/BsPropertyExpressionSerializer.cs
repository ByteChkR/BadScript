using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;

namespace BadScript.Utils.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            bool hasVal = s.DeserializeBool();
            BSExpression expr = null;

            if ( hasVal )
            {
                expr = s.DeserializeExpression();
            }

            string r = s.DeserializeString();

            return new BSPropertyExpression( SourcePosition.Unknown, expr, r );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSPropertyExpression expr = ( BSPropertyExpression )e;

            ret.SerializeOpCode( BSCompiledExpressionCode.PropertyAccessExpr );
            ret.SerializeBool( expr.Left != null );

            if ( expr.Left != null )
            {
                ret.SerializeExpression( expr.Left );
            }

            ret.SerializeString( expr.Right );
        }

        #endregion

    }

}

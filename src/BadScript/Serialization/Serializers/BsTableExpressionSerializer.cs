using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Value;

namespace BadScript.Serialization.Serializers
{

    public class BsTableExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.TableExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSTableExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            return new BSTableExpression( SourcePosition.Unknown, s.DeserializeNameMap( context ) );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            BSTableExpression expr = ( BSTableExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.TableExpr );
            ret.SerializeNameMap( expr.InitExpressions, context );
        }

        #endregion

    }

}

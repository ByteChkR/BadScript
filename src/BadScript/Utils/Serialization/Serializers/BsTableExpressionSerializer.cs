using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            return new BSTableExpression( SourcePosition.Unknown, s.DeserializeNameMap() );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSTableExpression expr = ( BSTableExpression ) e;
            ret.SerializeOpCode( BSCompiledExpressionCode.TableExpr );
            ret.SerializeNameMap( expr.InitExpressions );

        }

        #endregion
    }

}

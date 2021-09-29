using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Types;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsUsingExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize(BSCompiledExpressionCode code)
        {
            return code == BSCompiledExpressionCode.UsingDefExpr;
        }

        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSUsingExpression;
        }

        public override BSExpression Deserialize(BSCompiledExpressionCode code, Stream s)
        {
            string[] fn = s.DeserializeStringArray();
            return new BSUsingExpression(SourcePosition.Unknown, fn);
        }

        public override void Serialize(BSExpression e, Stream ret)
        {
            BSUsingExpression u = ( BSUsingExpression )e;
            ret.SerializeOpCode(BSCompiledExpressionCode.UsingDefExpr);
            ret.SerializeStringArray( u.FullName );
        }

        #endregion

    }

}
using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsBlockExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.CustomBlock;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSBlockExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            return new BSBlockExpression( s.DeserializeBlock(context) );
        }

        public override void Serialize( BSExpression expr, Stream s, BSSerializerContext context)
        {
            BSBlockExpression b = ( BSBlockExpression )expr;
            s.SerializeOpCode( BSCompiledExpressionCode.CustomBlock );
            s.SerializeBlock( b.Block, context );
        }

        #endregion

    }

}

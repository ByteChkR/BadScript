using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Serialization.Serializers
{

    public class BsEnumerableFunctionDefinitionExpressionSerializer : BSExpressionSerializer
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.FunctionEnumerableDefinitionExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSEnumerableFunctionDefinitionExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            string n = s.DeserializeString();
            bool g = s.DeserializeBool();
            BSFunctionParameter[] p = s.DeserializeFunctionParameters();
            BSExpression[] b = s.DeserializeBlock();

            return new BSEnumerableFunctionDefinitionExpression( SourcePosition.Unknown, n, g, p, b );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSEnumerableFunctionDefinitionExpression expr = ( BSEnumerableFunctionDefinitionExpression ) e;
            ret.SerializeOpCode( BSCompiledExpressionCode.FunctionEnumerableDefinitionExpr );
            ret.SerializeString( expr.Name );
            ret.SerializeBool( expr.Global );
            ret.SerializeFunctionParameters( expr.ArgNames );
            ret.SerializeBlock( expr.Block );

        }

        #endregion
    }

}

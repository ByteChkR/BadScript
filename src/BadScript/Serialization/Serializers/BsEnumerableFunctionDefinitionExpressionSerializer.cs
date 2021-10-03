using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block;

namespace BadScript.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            string n = s.DeserializeString( context );
            bool g = s.DeserializeBool();
            BSFunctionParameter[] p = s.DeserializeFunctionParameters( context );
            BSExpression[] b = s.DeserializeBlock( context );

            return new BSEnumerableFunctionDefinitionExpression( SourcePosition.Unknown, n, g, p, b );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            BSEnumerableFunctionDefinitionExpression expr = ( BSEnumerableFunctionDefinitionExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.FunctionEnumerableDefinitionExpr );
            ret.SerializeString( expr.Name, context );
            ret.SerializeBool( expr.Global );
            ret.SerializeFunctionParameters( expr.ArgNames, context );
            ret.SerializeBlock( expr.Block, context );
        }

        #endregion

    }

}

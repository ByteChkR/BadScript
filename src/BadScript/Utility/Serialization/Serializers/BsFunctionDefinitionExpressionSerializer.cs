using System.IO;
using System.Linq;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsFunctionDefinitionExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.FunctionDefinitionExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSFunctionDefinitionExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            string name = s.DeserializeString(context);
            bool global = s.DeserializeBool();
            BSFunctionParameter[] args = s.DeserializeFunctionParameters(context);

            BSExpression[] exprs = s.DeserializeBlock(context);

            return new BSFunctionDefinitionExpression(
                                                      SourcePosition.Unknown,
                                                      name,
                                                      args.ToArray(),
                                                      exprs.ToArray(),
                                                      global
                                                     );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSFunctionDefinitionExpression expr = ( BSFunctionDefinitionExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.FunctionDefinitionExpr );
            ret.SerializeString( expr.Name, context );
            ret.SerializeBool( expr.Global );
            ret.SerializeFunctionParameters( expr.ArgNames, context);

            ret.SerializeBlock( expr.Block, context);
        }

        #endregion

    }

}

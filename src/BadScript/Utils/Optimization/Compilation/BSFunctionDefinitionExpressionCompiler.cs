using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSFunctionDefinitionExpressionCompiler : BSExpressionCompiler
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            string name = s.DeserializeString();
            bool global = s.DeserializeBool();
            BSFunctionParameter[] args = s.DeserializeFunctionParameters();

            BSExpression[] exprs = s.DeserializeBlock();

            return new BSFunctionDefinitionExpression(
                SourcePosition.Unknown,
                name,
                args.ToArray(),
                exprs.ToArray(),
                global );
        }

        public override byte[] Serialize( BSExpression e )
        {
            BSFunctionDefinitionExpression expr = ( BSFunctionDefinitionExpression ) e;
            List < byte > ret = new List < byte >();
            ret.SerializeOpCode( BSCompiledExpressionCode.FunctionDefinitionExpr );
            ret.SerializeString( expr.Name );
            ret.SerializeBool( expr.Global );
            ret.SerializeFunctionParameters( expr.ArgNames );

            ret.SerializeBlock( expr.Block );

            return ret.ToArray();
        }

        #endregion
    }

}

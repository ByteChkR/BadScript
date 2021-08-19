using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSEnumerableFunctionDefinitionExpressionCompiler : BSExpressionCompiler
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

        public override byte[] Serialize( BSExpression e )
        {
            BSEnumerableFunctionDefinitionExpression expr = ( BSEnumerableFunctionDefinitionExpression ) e;
            List < byte > ret = new List < byte >();
            ret.SerializeOpCode( BSCompiledExpressionCode.FunctionEnumerableDefinitionExpr );
            ret.SerializeString( expr.Name );
            ret.SerializeBool( expr.Global );
            ret.SerializeFunctionParameters( expr.ArgNames );
            ret.SerializeBlock( expr.Block );

            return ret.ToArray();
        }

        #endregion
    }

}

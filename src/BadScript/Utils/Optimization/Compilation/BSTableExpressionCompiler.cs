using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSTableExpressionCompiler : BSExpressionCompiler
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

        public override byte[] Serialize( BSExpression e )
        {
            BSTableExpression expr = ( BSTableExpression ) e;
            List < byte > ret = new List < byte >();
            ret.SerializeOpCode( BSCompiledExpressionCode.TableExpr );
            ret.SerializeNameMap( expr.InitExpressions );

            return ret.ToArray();
        }

        #endregion
    }

}

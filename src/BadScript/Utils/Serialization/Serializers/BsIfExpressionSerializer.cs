using System.Collections.Generic;
using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BsIfExpressionSerializer : BSExpressionSerializer
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.IfExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSIfExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            Dictionary < BSExpression, BSExpression[] > map = s.DeserializeMap();
            BSExpression[] elseBlock = s.DeserializeBlock();

            return new BSIfExpression( SourcePosition.Unknown, map, elseBlock );
        }

        public override void Serialize(BSExpression e, Stream ret)
        {
            BSIfExpression expr = ( BSIfExpression ) e;
            ret.SerializeOpCode( BSCompiledExpressionCode.IfExpr );
            ret.SerializeMap( expr.ConditionMap );
            ret.SerializeBlock( expr.ElseBlock ?? new BSExpression[0] );
            
        }

        #endregion
    }

}

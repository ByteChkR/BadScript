using System;
using System.Collections.Generic;
using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utility.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            Dictionary < BSExpression, BSExpression[] > map = s.DeserializeMap(context);
            BSExpression[] elseBlock = s.DeserializeBlock(context);

            return new BSIfExpression( SourcePosition.Unknown, map, elseBlock );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSIfExpression expr = ( BSIfExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.IfExpr );
            ret.SerializeMap( expr.ConditionMap , context);
            ret.SerializeBlock( expr.ElseBlock ?? Array.Empty < BSExpression >(), context );
        }

        #endregion

    }

}

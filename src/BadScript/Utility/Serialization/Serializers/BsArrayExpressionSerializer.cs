﻿using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsArrayExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ArrayExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSArrayExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            return new BSArrayExpression( SourcePosition.Unknown, s.DeserializeBlock(context) );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSArrayExpression expr = ( BSArrayExpression )e;

            ret.SerializeOpCode( BSCompiledExpressionCode.ArrayExpr );
            ret.SerializeBlock( expr.InitExpressions , context);
        }

        #endregion

    }

}

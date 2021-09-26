using System;
using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utils.Serialization.Serializers
{

    public class BsContinueExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ContinueExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSContinueExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            return new BSContinueExpression( SourcePosition.Unknown );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            byte[] b = BitConverter.GetBytes( ( byte )BSCompiledExpressionCode.ContinueExpr );
            ret.Write( b, 0, b.Length );
        }

        #endregion

    }

}

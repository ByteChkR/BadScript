using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Types;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsNamespaceExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.NamespaceDefExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSNamespaceExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            string[] fn = s.DeserializeStringArray();
            BSExpression[] block = s.DeserializeBlock();

            return new BSNamespaceExpression( SourcePosition.Unknown, fn, block );
        }

        public override void Serialize( BSExpression expr, Stream s )
        {
            BSNamespaceExpression ns = ( BSNamespaceExpression )expr;
            s.SerializeOpCode( BSCompiledExpressionCode.NamespaceDefExpr );
            s.SerializeStringArray( ns.FullName );
            s.SerializeBlock( ns.Block );
        }

        #endregion

    }

}

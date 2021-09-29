using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsBreakExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize(BSCompiledExpressionCode code)
        {
            return code == BSCompiledExpressionCode.BreakExpr;
        }

        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSBreakExpression;
        }

        public override BSExpression Deserialize(BSCompiledExpressionCode code, Stream s)
        {
            return new BSBreakExpression(SourcePosition.Unknown);
        }

        public override void Serialize(BSExpression e, Stream ret)
        {
            ret.SerializeOpCode(BSCompiledExpressionCode.BreakExpr);
        }

        #endregion

    }

    public class BsNamespaceExpressionSerializer : BSExpressionSerializer
    {

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
            s.SerializeBlock(ns.Block);
        }

    }

    public class BsUsingExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize(BSCompiledExpressionCode code)
        {
            return code == BSCompiledExpressionCode.UsingDefExpr;
        }

        public override bool CanSerialize(BSExpression expr)
        {
            return expr is BSUsingExpression;
        }

        public override BSExpression Deserialize(BSCompiledExpressionCode code, Stream s)
        {
            string[] fn = s.DeserializeStringArray();
            return new BSUsingExpression(SourcePosition.Unknown, fn);
        }

        public override void Serialize(BSExpression e, Stream ret)
        {
            BSUsingExpression u = ( BSUsingExpression )e;
            ret.SerializeOpCode(BSCompiledExpressionCode.UsingDefExpr);
            ret.SerializeStringArray( u.FullName );
        }

        #endregion

    }

}

using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Value;

namespace BadScript.Serialization.Serializers
{

    public class BsFormattedStringExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ValueFormattedString;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSFormattedStringExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            string fmt = s.DeserializeString( context );
            int exprCount = s.DeserializeInt32();

            BSExpression[] exprs = new BSExpression[exprCount];

            for ( int i = 0; i < exprCount; i++ )
            {
                exprs[i] = s.DeserializeExpression( context );
            }

            return new BSFormattedStringExpression( SourcePosition.Unknown, fmt, exprs );
        }

        public override void Serialize( BSExpression expr, Stream s, BSSerializerContext context )
        {
            BSFormattedStringExpression e = ( BSFormattedStringExpression )expr;
            s.SerializeOpCode( BSCompiledExpressionCode.ValueFormattedString );
            s.SerializeString( e.FormatString, context );
            s.SerializeInt32( e.Args.Length );

            foreach ( BSExpression bsExpression in e.Args )
            {
                s.SerializeExpression( bsExpression, context );
            }
        }

        #endregion

    }

}

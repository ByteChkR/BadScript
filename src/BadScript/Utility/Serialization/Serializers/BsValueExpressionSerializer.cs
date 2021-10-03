using System.IO;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utility.Serialization.Serializers
{

    public class BsValueExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ValueDecimal ||
                   code == BSCompiledExpressionCode.ValueString ||
                   code == BSCompiledExpressionCode.ValueBoolean ||
                   code == BSCompiledExpressionCode.ValueNull;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSValueExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            if ( code == BSCompiledExpressionCode.ValueNull )
            {
                return new BSValueExpression( SourcePosition.Unknown, null );
            }
            else if ( code == BSCompiledExpressionCode.ValueDecimal )
            {
                return new BSValueExpression( SourcePosition.Unknown, s.DeserializeDecimal() );
            }
            else if ( code == BSCompiledExpressionCode.ValueString )
            {
                return new BSValueExpression( SourcePosition.Unknown, s.DeserializeString(context) );
            }
            else if ( code == BSCompiledExpressionCode.ValueBoolean )
            {
                return new BSValueExpression( SourcePosition.Unknown, s.DeserializeBool() );
            }

            throw new BSInvalidOperationException(
                                                  SourcePosition.Unknown,
                                                  "Can not DeserializeExpression Expression: " + code
                                                 );
        }

        public override void Serialize( BSExpression e, Stream b, BSSerializerContext context)
        {
            BSValueExpression expr = ( BSValueExpression )e;

            if ( expr.SourceValue is string s )
            {
                b.SerializeOpCode( BSCompiledExpressionCode.ValueString );
                b.SerializeString( s, context );
            }
            else if ( expr.SourceValue is decimal d )
            {
                b.SerializeOpCode( BSCompiledExpressionCode.ValueDecimal );
                b.SerializeDecimal( d );
            }
            else if ( expr.SourceValue is bool boo )
            {
                b.SerializeOpCode( BSCompiledExpressionCode.ValueBoolean );
                b.SerializeBool( boo );
            }
            else if ( expr.SourceValue == null )
            {
                b.SerializeOpCode( BSCompiledExpressionCode.ValueNull );
            }
        }

        #endregion

    }

}

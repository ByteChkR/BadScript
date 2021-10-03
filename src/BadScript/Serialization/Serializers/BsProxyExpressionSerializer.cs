using System.IO;

using BadScript.Exceptions;
using BadScript.Optimization;
using BadScript.Parser.Expressions;
using BadScript.Parser.Operators.Implementations;

namespace BadScript.Serialization.Serializers
{

    public class BsProxyExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.UnaryExpr ||
                   code == BSCompiledExpressionCode.BinaryExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSProxyExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            if ( code == BSCompiledExpressionCode.UnaryExpr )
            {
                string key = s.DeserializeString( context );
                string implKey = s.DeserializeString( context );
                string sig = s.DeserializeString( context );
                int argc = s.DeserializeInt32();
                BSUnaryOperatorMetaData um = new BSUnaryOperatorMetaData( key, implKey, sig, argc );

                return new BSProxyExpression( SourcePosition.Unknown, um.MakeFunction(), um );
            }
            else if ( code == BSCompiledExpressionCode.BinaryExpr )
            {
                string key = s.DeserializeString( context );
                string sig = s.DeserializeString( context );
                int argc = s.DeserializeInt32();
                BSBinaryOperatorMetaData um = new BSBinaryOperatorMetaData( key, sig, argc );

                return new BSProxyExpression( SourcePosition.Unknown, um.MakeFunction(), um );
            }

            throw new BSInvalidOperationException(
                                                  SourcePosition.Unknown,
                                                  "Can not DeserializeExpression Expression: " + code
                                                 );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context )
        {
            BSProxyExpression expr = ( BSProxyExpression )e;

            if ( expr.ProxyMetaData == null )
            {
                throw new BSInvalidOperationException(
                                                      expr.Object.Position,
                                                      "Can not Serialize Proxy Expression. No meta data provided"
                                                     );
            }

            if ( expr.ProxyMetaData is BSBinaryOperatorMetaData bm )
            {
                ret.SerializeOpCode( BSCompiledExpressionCode.BinaryExpr );
                ret.SerializeString( bm.OperatorKey, context );
                ret.SerializeString( bm.Signature, context );
                ret.SerializeInt32( bm.ArgumentCount );
            }
            else if ( expr.ProxyMetaData is BSUnaryOperatorMetaData um )
            {
                ret.SerializeOpCode( BSCompiledExpressionCode.UnaryExpr );
                ret.SerializeString( um.OperatorKey, context );
                ret.SerializeString( um.ImplementationOperatorKey, context );
                ret.SerializeString( um.Signature, context );
                ret.SerializeInt32( um.ArgumentCount );
            }
            else if ( expr.ProxyMetaData is BSExpressionOptimizerMetaData om )
            {
                ret.SerializeOpCode( om.ExpressionCode );

                if ( om.ExpressionCode == BSCompiledExpressionCode.ValueString )
                {
                    ret.SerializeString( ( string )om.Value, context );
                }
                else if ( om.ExpressionCode == BSCompiledExpressionCode.ValueDecimal )
                {
                    ret.SerializeDecimal( ( decimal )om.Value );
                }
                else if ( om.ExpressionCode == BSCompiledExpressionCode.ValueBoolean )
                {
                    ret.SerializeBool( ( bool )om.Value );
                }
            }
            else
            {
                throw new BSInvalidOperationException(
                                                      expr.Object.Position,
                                                      "Can not Serialize Proxy Expression. Invalid meta data provided"
                                                     );
            }
        }

        #endregion

    }

}

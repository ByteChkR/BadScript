﻿using System.Collections.Generic;
using System.IO;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Operators.Implementations;

namespace BadScript.Utils.Optimization.Compilation
{

    public class BSProxyExpressionCompiler : BSExpressionCompiler
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            if ( code == BSCompiledExpressionCode.UnaryExpr )
            {
                string key = s.DeserializeString();
                string implKey = s.DeserializeString();
                string sig = s.DeserializeString();
                int argc = s.DeserializeInt32();
                BSUnaryOperatorMetaData um = new BSUnaryOperatorMetaData( key, implKey, sig, argc );

                return new BSProxyExpression( SourcePosition.Unknown, um.MakeFunction(), um );
            }
            else if ( code == BSCompiledExpressionCode.BinaryExpr )
            {
                string key = s.DeserializeString();
                string sig = s.DeserializeString();
                int argc = s.DeserializeInt32();
                BSBinaryOperatorMetaData um = new BSBinaryOperatorMetaData( key, sig, argc );

                return new BSProxyExpression( SourcePosition.Unknown, um.MakeFunction(), um );
            }

            throw new BSInvalidOperationException(
                SourcePosition.Unknown,
                "Can not DeserializeExpression Expression: " + code );
        }

        public override byte[] Serialize( BSExpression e )
        {
            BSProxyExpression expr = ( BSProxyExpression ) e;

            if ( expr.ProxyMetaData == null )
            {
                throw new BSInvalidOperationException(
                    expr.Object.Position,
                    "Can not Serialize Proxy Expression. No meta data provided" );
            }

            List < byte > ret = new List < byte >();

            if ( expr.ProxyMetaData is BSBinaryOperatorMetaData bm )
            {
                ret.SerializeOpCode( BSCompiledExpressionCode.BinaryExpr );
                ret.SerializeString( bm.OperatorKey );
                ret.SerializeString( bm.Signature );
                ret.SerializeInt32( bm.ArgumentCount );
            }
            else if ( expr.ProxyMetaData is BSUnaryOperatorMetaData um )
            {
                ret.SerializeOpCode( BSCompiledExpressionCode.UnaryExpr );
                ret.SerializeString( um.OperatorKey );
                ret.SerializeString( um.ImplementationOperatorKey );
                ret.SerializeString( um.Signature );
                ret.SerializeInt32( um.ArgumentCount );
            }
            else if ( expr.ProxyMetaData is BSExpressionOptimizerMetaData om )
            {
                ret.SerializeOpCode( om.ExpressionCode );

                if ( om.ExpressionCode == BSCompiledExpressionCode.ValueString )
                {
                    ret.SerializeString( ( string ) om.Value );
                }
                else if ( om.ExpressionCode == BSCompiledExpressionCode.ValueDecimal )
                {
                    ret.SerializeDecimal( ( decimal ) om.Value );
                }
            }
            else
            {
                throw new BSInvalidOperationException(
                    expr.Object.Position,
                    "Can not Serialize Proxy Expression. Invalid meta data provided" );
            }

            return ret.ToArray();
        }

        #endregion
    }

}

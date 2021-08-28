using System.IO;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;

namespace BadScript.Utils.Serialization.Serializers
{

    public class BsForeachExpressionSerializer : BSExpressionSerializer
    {
        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.ForEachExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSForeachExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            int vCount = s.DeserializeInt32();
            string[] vars = new string[vCount];

            for ( int i = 0; i < vCount; i++ )
            {
                vars[i] = s.DeserializeString();
            }

            BSExpression enumerator = s.DeserializeExpression();
            BSExpression[] block = s.DeserializeBlock();

            return new BSForeachExpression( SourcePosition.Unknown, vars, enumerator, block );
        }

        public override void Serialize( BSExpression e, Stream ret )
        {
            BSForeachExpression expr = ( BSForeachExpression ) e;
            ret.SerializeOpCode( BSCompiledExpressionCode.ForEachExpr );
            ret.SerializeInt32( expr.Vars.Length );

            foreach ( string exprVar in expr.Vars )
            {
                ret.SerializeString( exprVar );
            }

            ret.SerializeExpression( expr.Enumerator );
            ret.SerializeBlock( expr.Block );

        }

        #endregion
    }

}

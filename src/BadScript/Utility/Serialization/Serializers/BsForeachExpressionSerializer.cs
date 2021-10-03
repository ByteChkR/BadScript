using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;

namespace BadScript.Utility.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context)
        {
            int vCount = s.DeserializeInt32();
            string[] vars = new string[vCount];

            for ( int i = 0; i < vCount; i++ )
            {
                vars[i] = s.DeserializeString(context);
            }

            BSExpression enumerator = s.DeserializeExpression(context);
            BSExpression[] block = s.DeserializeBlock(context);

            return new BSForeachExpression( SourcePosition.Unknown, vars, enumerator, block );
        }

        public override void Serialize( BSExpression e, Stream ret, BSSerializerContext context)
        {
            BSForeachExpression expr = ( BSForeachExpression )e;
            ret.SerializeOpCode( BSCompiledExpressionCode.ForEachExpr );
            ret.SerializeInt32( expr.Vars.Length );

            foreach ( string exprVar in expr.Vars )
            {
                ret.SerializeString( exprVar, context);
            }

            ret.SerializeExpression( expr.Enumerator, context);
            ret.SerializeBlock( expr.Block, context);
        }

        #endregion

    }

}

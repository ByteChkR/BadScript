using System.IO;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Types;
using BadScript.Parser.Expressions.Implementations.Value;

namespace BadScript.Serialization.Serializers
{

    public class BsNewClassExpressionSerializer : BSExpressionSerializer
    {

        #region Public

        public override bool CanDeserialize( BSCompiledExpressionCode code )
        {
            return code == BSCompiledExpressionCode.NewClassExpr;
        }

        public override bool CanSerialize( BSExpression expr )
        {
            return expr is BSNewInstanceExpression;
        }

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s, BSSerializerContext context )
        {
            BSExpression expr = s.DeserializeExpression( context );

            if ( expr is BSInvocationExpression invoc )
            {
                return new BSNewInstanceExpression( SourcePosition.Unknown, invoc );
            }

            throw new BSSerializerException(
                                            $"Invalid BSExpression for NewInstance Operator. Expected invocation but got '{expr.GetType()}'"
                                           );
        }

        public override void Serialize( BSExpression expr, Stream s, BSSerializerContext context )
        {
            BSNewInstanceExpression e = ( BSNewInstanceExpression )expr;
            s.SerializeOpCode( BSCompiledExpressionCode.NewClassExpr );
            s.SerializeExpression( e.Name, context );
        }

        #endregion

    }

}

using System.IO;

using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Utility.Serialization.Serializers
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

        public override BSExpression Deserialize( BSCompiledExpressionCode code, Stream s )
        {
            BSExpression expr = s.DeserializeExpression();

            if ( expr is BSInvocationExpression invoc )
            {
                return new BSNewInstanceExpression( SourcePosition.Unknown, invoc );
            }

            throw new BSSerializerException(
                                            $"Invalid BSExpression for NewInstance Operator. Expected invocation but got '{expr.GetType()}'"
                                           );
        }

        public override void Serialize( BSExpression expr, Stream s )
        {
            BSNewInstanceExpression e = ( BSNewInstanceExpression )expr;
            s.SerializeOpCode( BSCompiledExpressionCode.NewClassExpr );
            s.SerializeExpression( e.Name );
        }

        #endregion

    }

}

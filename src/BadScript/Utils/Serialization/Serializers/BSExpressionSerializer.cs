using System.IO;

using BadScript.Common.Expressions;

namespace BadScript.Utils.Serialization.Serializers
{

    public abstract class BSExpressionSerializer
    {

        #region Public

        public abstract bool CanDeserialize( BSCompiledExpressionCode code );

        public abstract bool CanSerialize( BSExpression expr );

        public abstract BSExpression Deserialize( BSCompiledExpressionCode code, Stream s );

        public abstract void Serialize( BSExpression expr, Stream s );

        #endregion

    }

}

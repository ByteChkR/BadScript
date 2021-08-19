using System.IO;
using BadScript.Common.Expressions;

namespace BadScript.Utils.Optimization.Compilation
{

    public abstract class BSExpressionCompiler
    {
        #region Public

        public abstract bool CanDeserialize( BSCompiledExpressionCode code );

        public abstract bool CanSerialize( BSExpression expr );

        public abstract BSExpression Deserialize( BSCompiledExpressionCode code, Stream s );

        public abstract byte[] Serialize( BSExpression expr );

        #endregion
    }

}

using System.IO;
using BadScript.Common.Expressions;

namespace BadScript.Utils.Optimization.Compilation
{

    public abstract class BSExpressionCompiler
    {
        public abstract bool CanSerialize(BSExpression expr);
        public abstract byte[] Serialize(BSExpression expr);

        public abstract bool CanDeserialize(BSCompiledExpressionCode code);
        public abstract BSExpression Deserialize(BSCompiledExpressionCode code, Stream s );
    }

}

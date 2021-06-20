using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Expressions.Value
{

    public class BSArrayExpression : BSExpression
    {

        #region Public

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            return new EngineRuntimeArray();
        }

        #endregion

    }

}

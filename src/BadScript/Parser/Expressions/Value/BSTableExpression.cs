using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Expressions.Value
{

    public class BSTableExpression : BSExpression
    {

        #region Public

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            return new EngineRuntimeTable();
        }

        #endregion

    }

}

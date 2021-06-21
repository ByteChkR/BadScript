using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSArrayExpression : BSExpression
    {

        #region Public

        public override ABSObject Execute( BSScope scope )
        {
            return new BSArray();
        }

        #endregion

    }

}

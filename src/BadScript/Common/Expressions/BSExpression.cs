using BadScript.Common.Runtime;
using BadScript.Common.Types;

namespace BadScript.Common.Expressions
{

    public abstract class BSExpression
    {
        #region Public

        public abstract ABSObject Execute( BSScope scope );

        #endregion
    }

}

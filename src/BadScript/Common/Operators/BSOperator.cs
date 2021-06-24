using BadScript.Common.Expressions;

namespace BadScript.Common.Operators
{

    public abstract class BSOperator
    {
        public abstract string OperatorKey { get; }

        #region Public

        public abstract BSExpression Parse( BSExpression left, BSParser parser );

        #endregion
    }

}

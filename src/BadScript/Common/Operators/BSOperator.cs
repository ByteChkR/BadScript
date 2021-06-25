using BadScript.Common.Expressions;

namespace BadScript.Common.Operators
{

    public abstract class BSOperator
    {
        public abstract string OperatorKey { get; }

        public abstract int Preceedence { get; }

        #region Public

        public abstract BSExpression Parse( BSExpression left, BSParser parser );

        #endregion
    }

}

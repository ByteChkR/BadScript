using BadScript.Common.Expressions;

namespace BadScript.Common.Operators
{
    /// <summary>
    /// Abstract base for all Operators used.
    /// </summary>
    public abstract class BSOperator
    {

        /// <summary>
        /// The Key of the Operator(that gets matched when parsing an expression)
        /// </summary>
        public abstract string OperatorKey { get; }

        /// <summary>
        /// The Operator precedence
        /// </summary>
        public abstract int Preceedence { get; }

        #region Public

        /// <summary>
        /// Parses the Operator
        /// </summary>
        /// <param name="left">Left Side of the Operator</param>
        /// <param name="parser">The Parser used to parse the left side.(is used to parse a right side if needed)</param>
        /// <returns></returns>
        public abstract BSExpression Parse( BSExpression left, BSParser parser );

        #endregion

    }

}

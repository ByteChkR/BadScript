using BadScript.Common.Types;

namespace BadScript.Common.OperatorImplementations
{
    /// <summary>
    /// Abstract base for all BSOperator Implementations used.
    /// </summary>
    public abstract class ABSOperatorImplementation
    {

        /// <summary>
        /// The Key of the Operator(that gets matched when finding an operator expression)
        /// </summary>
        public readonly string OperatorKey;

        #region Public

        /// <summary>
        /// Returns true if the operator can be applied to the specified arguments
        /// </summary>
        /// <param name="args">The Arguments of the Operator</param>
        /// <returns></returns>
        public abstract bool IsCorrectImplementation( ABSObject[] args );

        /// <summary>
        /// Executes an Operator on the specified arguments
        /// </summary>
        /// <param name="arg">The operator Arguments</param>
        /// <returns>Return of the Operator Execution</returns>
        public ABSObject ExecuteOperator( ABSObject[] arg )
        {
            return Execute( arg );
        }

        #endregion

        #region Protected

        protected ABSOperatorImplementation( string key )
        {
            OperatorKey = key;
        }

        protected abstract ABSObject Execute( ABSObject[] args );

        #endregion

    }

}

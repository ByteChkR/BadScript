namespace BadScript.Common.Expressions.Implementations.Binary
{

    public abstract class BSBinaryExpression : BSExpression
    {
        protected readonly BSExpression Left;
        protected readonly BSExpression Right;

        #region Protected

        protected BSBinaryExpression( BSExpression left, BSExpression right )
        {
            Left = left;
            Right = right;
        }

        #endregion
    }

}

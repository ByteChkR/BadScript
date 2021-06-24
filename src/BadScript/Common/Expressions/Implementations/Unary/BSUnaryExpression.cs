namespace BadScript.Common.Expressions.Implementations.Unary
{

    public abstract class BSUnaryExpression : BSExpression
    {
        protected readonly BSExpression Left;

        #region Protected

        protected BSUnaryExpression( BSExpression left )
        {
            Left = left;
        }

        #endregion
    }

}

namespace BadScript.Parser.Expressions.Unary
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

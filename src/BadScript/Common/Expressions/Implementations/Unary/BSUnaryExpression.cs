namespace BadScript.Common.Expressions.Implementations.Unary
{

    public abstract class BSUnaryExpression : BSExpression
    {
        protected readonly BSExpression Left;

        #region Protected

        protected BSUnaryExpression( SourcePosition srcPos, BSExpression left ) : base( srcPos )
        {
            Left = left;
        }

        #endregion
    }

}

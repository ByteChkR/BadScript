namespace BadScript.Common.Expressions.Implementations.Binary
{

    public abstract class BSBinaryExpression : BSExpression
    {
        public BSExpression Left;
        public BSExpression Right;

        #region Protected

        protected BSBinaryExpression( SourcePosition srcPos, BSExpression left, BSExpression right ) : base( srcPos )
        {
            Left = left;
            Right = right;
        }

        #endregion
    }

}

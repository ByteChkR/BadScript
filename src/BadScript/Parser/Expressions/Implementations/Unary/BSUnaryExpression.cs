namespace BadScript.Parser.Expressions.Implementations.Unary
{

    public abstract class BSUnaryExpression : BSExpression
    {

        public BSExpression Left;

        #region Protected

        protected BSUnaryExpression( SourcePosition srcPos, BSExpression left ) : base( srcPos )
        {
            Left = left;
        }

        #endregion

    }

}

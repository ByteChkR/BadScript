using BadScript.Runtime;

namespace BadScript.Parser.Expressions.Value
{

    public class BSReturnExpression : BSExpression
    {

        public BSExpression Left { get; private set; }

        #region Public

        public BSReturnExpression( BSExpression left )
        {
            Left = left;
        }

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            BSRuntimeObject o = Left.Execute( scope );

            if ( o is BSRuntimeReference reference )
            {
                o = reference.Get();
            }

            scope.SetReturnValue( o );

            return o;
        }

        #endregion

    }

}

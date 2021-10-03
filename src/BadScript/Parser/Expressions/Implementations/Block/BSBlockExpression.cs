using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Block
{

    public class BSBlockExpression : BSExpression
    {

        public BSExpression[] Block;

        public override bool IsConstant => false;

        #region Public

        public BSBlockExpression( BSExpression[] block ) : base( SourcePosition.Unknown )
        {
            Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            foreach ( BSExpression bsExpression in Block )
            {
                bsExpression.Execute( scope );

                if ( scope.BreakExecution )
                {
                    break;
                }
            }

            return BSObject.Null;
        }

        #endregion

    }

}

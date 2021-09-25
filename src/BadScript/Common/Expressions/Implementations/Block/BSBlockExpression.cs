using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSBlockExpression : BSExpression
    {

        public BSExpression[] Block;
        public BSBlockExpression( BSExpression[] block ) : base( SourcePosition.Unknown )
        {
            Block = block;
        }

        public override bool IsConstant => false;

        public override ABSObject Execute( BSScope scope )
        {
            foreach ( BSExpression bsExpression in Block)
            {
                bsExpression.Execute( scope );

                if ( scope.BreakExecution )
                    break;
            }

            return BSObject.Null;
        }

    }

}

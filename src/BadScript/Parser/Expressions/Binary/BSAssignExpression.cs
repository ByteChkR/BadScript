using System;

using BadScript.Runtime;

namespace BadScript.Parser.Expressions.Binary
{

    public class BSAssignExpression : BSBinaryExpression
    {

        #region Public

        public BSAssignExpression( BSExpression left, BSExpression right ) : base( left, right )
        {
        }

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            BSRuntimeObject right = Right.Execute( scope );

            if ( right is BSRuntimeReference rRef )
            {
                right = rRef.Get();
            }

            BSRuntimeObject left = Left.Execute( scope );

            if ( left is BSRuntimeReference lRef )
            {
                lRef.Assign( right );
            }
            else
            {
                throw new Exception( $"{left} can not be assigned to." );
            }
            

            return right;
        }

        #endregion

    }

}

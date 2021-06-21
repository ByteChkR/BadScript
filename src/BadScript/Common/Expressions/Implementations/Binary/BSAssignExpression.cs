using System;

using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Binary
{

    public class BSAssignExpression : BSBinaryExpression
    {

        #region Public

        public BSAssignExpression( BSExpression left, BSExpression right ) : base( left, right )
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject right = Right.Execute( scope ).ResolveReference();

            ABSObject left = Left.Execute( scope );

            if ( left is ABSReference lRef )
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

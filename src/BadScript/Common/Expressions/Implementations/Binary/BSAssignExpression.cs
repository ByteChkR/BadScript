using BadScript.Common.Exceptions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Binary
{

    public class BSAssignExpression : BSBinaryExpression
    {
        public override bool IsConstant => Left.IsConstant && Right.IsConstant;

        #region Public

        public BSAssignExpression( SourcePosition srcPos, BSExpression left, BSExpression right ) : base(
            srcPos,
            left,
            right )
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
                throw new BSInvalidTypeException(
                    m_Position,
                    "Expected Assignable Reference",
                    left,
                    "Reference"
                );
            }

            return right;
        }

        #endregion
    }

}

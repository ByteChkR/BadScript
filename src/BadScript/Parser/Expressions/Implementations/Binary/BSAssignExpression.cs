using BadScript.Exceptions;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.References;

namespace BadScript.Parser.Expressions.Implementations.Binary
{

    public class BSAssignExpression : BSBinaryExpression
    {

        public override bool IsConstant => Left.IsConstant && Right.IsConstant;

        #region Public

        public BSAssignExpression( SourcePosition srcPos, BSExpression left, BSExpression right ) : base(
             srcPos,
             left,
             right
            )
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

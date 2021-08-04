using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Binary;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Access
{

    public class BSArrayAccessExpression : BSBinaryExpression
    {
        public override bool IsConstant => Left.IsConstant && Right.IsConstant;

        #region Public

        public BSArrayAccessExpression( SourcePosition srcPos, BSExpression left, BSExpression arg ) : base(
            srcPos,
            left,
            arg )
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject obj = Left.Execute( scope ).ResolveReference();

            ABSObject i = Right.Execute( scope ).ResolveReference();

            if ( obj is ABSTable t )
            {
                return t.GetElement( i );
            }

            if ( obj is ABSArray a )
            {
                if ( i.TryConvertDecimal( out decimal d ) )
                {
                    return a.GetElement( ( int ) d );
                }
            }

            throw new BSInvalidTypeException(
                m_Position,
                "Expected Array",
                obj,
                "Table"
            );

        }

        #endregion
    }

}

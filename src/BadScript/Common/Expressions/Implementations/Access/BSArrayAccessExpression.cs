using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Unary;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Access
{

    public class BSArrayAccessExpression : BSUnaryExpression
    {
        private BSExpression Parameter;

        #region Public

        public BSArrayAccessExpression( SourcePosition srcPos, BSExpression left, BSExpression arg ) : base(
            srcPos,
            left )
        {
            Parameter = arg;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject obj = Left.Execute( scope ).ResolveReference();

            ABSObject i = Parameter.Execute( scope ).ResolveReference();

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

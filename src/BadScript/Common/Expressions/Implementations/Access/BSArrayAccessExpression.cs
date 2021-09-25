using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Binary;
using BadScript.Common.OperatorImplementations;
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
             arg
            )
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject obj = Left.Execute( scope ).ResolveReference();

            ABSObject i = Right.Execute( scope ).ResolveReference();

            ABSOperatorImplementation impl =
                BSOperatorImplementationResolver.ResolveImplementation( "[]", new[] { obj, i }, true );

            return impl.ExecuteOperator( new[] { obj, i } );

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

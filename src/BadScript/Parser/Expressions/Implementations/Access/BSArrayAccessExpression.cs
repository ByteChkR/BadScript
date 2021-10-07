using BadScript.Exceptions;
using BadScript.Parser.Expressions.Implementations.Binary;
using BadScript.Parser.OperatorImplementations;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.Expressions.Implementations.Access
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

            if(BSEngineSettings.ENABLE_CORE_FAST_TRACK)
            {
                if(obj is BSArray arr)
                {
                    return arr.GetElement((int)i.ConvertDecimal());
                }
                if(obj is BSTable table)
                {
                    return table.GetElement(i);
                }
            }

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

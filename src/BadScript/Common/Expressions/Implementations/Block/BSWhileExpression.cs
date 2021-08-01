using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSWhileExpression : BSExpression
    {
        private BSExpression m_Condition;
        private BSExpression[] m_Block;

        #region Public

        public BSWhileExpression( SourcePosition srcPos, BSExpression condition, BSExpression[] block ) : base( srcPos )
        {
            m_Condition = condition;
            m_Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSScope funcScope = new BSScope( BSScopeFlags.Loop, scope );
            ABSObject o = m_Condition.Execute( funcScope ).ResolveReference();

            while ( o.TryConvertBool( out bool d ) && d )
            {

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    funcScope,
                    m_Block,
                    new string[0],
                    new ABSObject[0]
                );

                if ( funcScope.Flags == BSScopeFlags.Continue )
                {
                    funcScope.SetFlag( BSScopeFlags.None );
                }

                if ( ret != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, ret );

                    break;
                }
                else if ( funcScope.Flags == BSScopeFlags.Break )
                {
                    break;
                }

                o = m_Condition.Execute( funcScope ).ResolveReference();
            }

            return BSObject.Null;
        }

        #endregion
    }

}

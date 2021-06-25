using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Block.ForEach
{

    public class BSForExpression : BSExpression
    {
        private BSExpression m_CounterDefinition;
        private BSExpression m_CounterCondition;
        private BSExpression m_CounterIncrement;
        private BSExpression[] m_Block;

        #region Public

        public BSForExpression( BSExpression cDef, BSExpression cCond, BSExpression cInc, BSExpression[] block )
        {
            m_CounterCondition = cCond;
            m_CounterDefinition = cDef;
            m_CounterIncrement = cInc;
            m_Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSScope forScope = new BSScope( BSScopeFlags.Loop, scope );
            m_CounterDefinition.Execute( forScope );
            ABSObject c = m_CounterCondition.Execute( forScope ).ResolveReference();

            while ( c.TryConvertBool( out bool d ) && d )
            {

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    forScope,
                    m_Block,
                    new string[0],
                    new ABSObject[0]
                );

                if ( forScope.Flags == BSScopeFlags.Continue )
                {
                    scope.SetFlag( BSScopeFlags.None );
                }

                if ( ret != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, ret );

                    break;
                }
                else if ( forScope.Flags == BSScopeFlags.Break )
                {
                    break;
                }

                m_CounterIncrement.Execute( forScope );

                c = m_CounterCondition.Execute( forScope ).ResolveReference();

            }

            return new BSObject( null );
        }

        #endregion
    }

}

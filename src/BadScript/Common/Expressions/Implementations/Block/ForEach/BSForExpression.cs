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
            BSScope forScope = new BSScope( scope );
            m_CounterDefinition.Execute( forScope );
            ABSObject c = m_CounterCondition.Execute( forScope ).ResolveReference();

            while ( c.TryConvertBool( out bool d ) && d )
            {
                BSScope funcScope = new BSScope( scope );

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    funcScope,
                    m_Block,
                    new string[0],
                    new ABSObject[0]
                );

                if ( ret != null )
                {
                    scope.SetReturnValue( ret );

                    break;
                }

                m_CounterIncrement.Execute( scope );

                c = m_CounterCondition.Execute( scope ).ResolveReference();

            }

            return new BSObject( null );
        }

        #endregion
    }

}

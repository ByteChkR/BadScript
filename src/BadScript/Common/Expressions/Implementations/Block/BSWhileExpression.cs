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

        public BSWhileExpression( BSExpression condition, BSExpression[] block )
        {
            m_Condition = condition;
            m_Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject o = m_Condition.Execute( scope ).ResolveReference();

            while ( o.TryConvertBool( out bool d ) && d )
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

                o = m_Condition.Execute( scope ).ResolveReference();
            }

            return new BSObject( null );
        }

        #endregion

    }

}

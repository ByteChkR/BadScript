using System.Collections.Generic;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSIfExpression : BSExpression
    {
        private Dictionary < BSExpression, BSExpression[] > m_ConditionMap;
        private BSExpression[] m_ElseBlock;

        #region Public

        public BSIfExpression( Dictionary < BSExpression, BSExpression[] > conditions, BSExpression[] elseBlock = null )
        {
            m_ConditionMap = conditions;
            m_ElseBlock = elseBlock;
        }

        public override ABSObject Execute( BSScope scope )
        {
            foreach ( KeyValuePair < BSExpression, BSExpression[] > keyValuePair in m_ConditionMap )
            {
                ABSObject o = keyValuePair.Key.Execute( scope ).ResolveReference();
                bool d = o.ConvertBool();

                if ( d )
                {
                    BSScope funcScope = new BSScope(BSScopeFlags.IfBlock, scope );

                    ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                        funcScope,
                        keyValuePair.Value,
                        new string[0],
                        new ABSObject[0]
                    );

                    if ( ret != null )
                    {
                        scope.SetFlag(BSScopeFlags.Return, ret );
                    }else if ( funcScope.BreakExecution )
                        scope.SetFlag( funcScope.Flags );

                    return new BSObject( null );
                }

            }

            if ( m_ElseBlock != null )
            {
                BSScope elseScope = new BSScope(BSScopeFlags.IfBlock, scope );

                ABSObject elseR = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    elseScope,
                    m_ElseBlock,
                    new string[0],
                    new ABSObject[0]
                );

                if ( elseR != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, elseR );
                }
                else
                {
                    scope.SetFlag( elseScope.Flags );
                }
            }

            return new BSObject( null );
        }

        #endregion
    }

}

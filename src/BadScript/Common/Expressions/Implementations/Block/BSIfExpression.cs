using System;
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

                if ( o.TryConvertBool( out bool d ) )
                {
                    if ( d )
                    {
                        BSScope funcScope = new BSScope( scope );

                        ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                            funcScope,
                            keyValuePair.Value,
                            new string[0],
                            new ABSObject[0]
                        );

                        if ( ret != null )
                        {
                            scope.SetReturnValue( ret );
                        }

                        return new BSObject( null );
                    }
                }
                else
                {
                    throw new Exception( "invalid if expression" );
                }
            }

            if ( m_ElseBlock != null )
            {
                BSScope elseScope = new BSScope( scope );

                ABSObject elseR = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    elseScope,
                    m_ElseBlock,
                    new string[0],
                    new ABSObject[0]
                );

                if ( elseR != null )
                {
                    scope.SetReturnValue( elseR );
                }
            }

            return new BSObject( null );
        }

        #endregion
    }

}

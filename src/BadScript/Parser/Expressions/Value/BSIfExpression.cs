using System;
using System.Collections.Generic;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Expressions.Value
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

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            foreach ( KeyValuePair < BSExpression, BSExpression[] > keyValuePair in m_ConditionMap )
            {
                BSRuntimeObject o = keyValuePair.Key.Execute( scope );

                if ( o is BSRuntimeReference r )
                {
                    o = r.Get();
                }

                if ( o.TryConvertBool( out bool d ) )
                {
                    if ( d )
                    {
                        BSEngineScope funcScope = new BSEngineScope( scope );

                        BSRuntimeObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                             funcScope,
                             keyValuePair.Value,
                             new string[0],
                             new BSRuntimeObject[0]
                            );

                        if ( ret != null )
                        {
                            scope.SetReturnValue( ret );
                        }

                        return new EngineRuntimeObject( null );
                    }
                }
                else
                {
                    throw new Exception( "invalid if expression" );
                }
            }

            if ( m_ElseBlock != null )
            {
                BSEngineScope elseScope = new BSEngineScope( scope );

                BSRuntimeObject elseR = BSFunctionDefinitionExpression.InvokeBlockFunction(
                     elseScope,
                     m_ElseBlock,
                     new string[0],
                     new BSRuntimeObject[0]
                    );

                if ( elseR != null )
                {
                    scope.SetReturnValue( elseR );
                }
            }

            return new EngineRuntimeObject( null );
        }

        #endregion

    }

}

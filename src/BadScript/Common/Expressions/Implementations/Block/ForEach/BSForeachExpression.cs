using System;
using System.Collections.Generic;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Block.ForEach
{

    public class BSForeachExpression : BSExpression
    {
        private string[] m_Vars;
        private BSExpression[] m_Block;
        private BSExpression m_Enumerator;

        #region Public

        public BSForeachExpression( string[] vars, BSExpression enumExpr, BSExpression[] block )
        {
            m_Vars = vars;
            m_Enumerator = enumExpr;
            m_Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSScope foreachScope = new BSScope( scope );
            ABSObject eObj = m_Enumerator.Execute( foreachScope ).ResolveReference();

            if ( eObj is IEnumerable < IForEachIteration > forEach )
            {
                foreach ( IForEachIteration iter in forEach )
                {
                    ABSObject[] objs = iter.GetObjects();

                    for ( int i = 0; i < m_Vars.Length; i++ )
                    {
                        foreachScope.AddLocalVar( m_Vars[i], objs.Length > i ? objs[i] : new BSObject( null ) );
                    }

                    ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                        foreachScope,
                        m_Block,
                        new string[0],
                        new ABSObject[0]
                    );

                    if ( ret != null )
                    {
                        scope.SetReturnValue( ret );

                        break;
                    }
                }
            }
            else
            {
                throw new Exception( $"Object '{eObj}' can not be Enumerated" );
            }

            return new BSObject( null );
        }

        #endregion
    }

}

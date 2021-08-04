﻿using System.Collections.Generic;
using BadScript.Common.Exceptions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Block.ForEach
{

    public class BSForeachExpression : BSExpression
    {
        public BSExpression[] Block;
        public BSExpression Enumerator;
        private string[] m_Vars;

        public override bool IsConstant => false;

        #region Public

        public BSForeachExpression(
            SourcePosition srcPos,
            string[] vars,
            BSExpression enumExpr,
            BSExpression[] block ) : base( srcPos )
        {
            m_Vars = vars;
            Enumerator = enumExpr;
            Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSScope foreachScope = new BSScope( BSScopeFlags.Loop, scope );
            ABSObject eObj = Enumerator.Execute( foreachScope ).ResolveReference();

            if ( eObj is IEnumerable < IForEachIteration > forEach )
            {
                foreach ( IForEachIteration iter in forEach )
                {
                    ABSObject[] objs = iter.GetObjects();

                    for ( int i = 0; i < m_Vars.Length; i++ )
                    {
                        foreachScope.AddLocalVar( m_Vars[i], objs.Length > i ? objs[i] : BSObject.Null );
                    }

                    ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                        foreachScope,
                        Block,
                        new string[0],
                        new ABSObject[0]
                    );

                    if ( foreachScope.Flags == BSScopeFlags.Continue )
                    {
                        scope.SetFlag( BSScopeFlags.None );
                    }

                    if ( ret != null )
                    {
                        scope.SetFlag( BSScopeFlags.Return, ret );

                        break;
                    }
                    else if ( foreachScope.BreakExecution )
                    {
                        break;
                    }
                }
            }
            else
            {
                throw new BSInvalidTypeException(
                    m_Position,
                    "Expected Enumerable Object",
                    eObj,
                    "Table",
                    "Array"
                );
            }

            return BSObject.Null;
        }

        #endregion
    }

}

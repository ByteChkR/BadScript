using System;
using System.Collections.Generic;

using BadScript.Exceptions;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.Expressions.Implementations.Block.ForEach
{

    public class BSForeachExpression : BSExpression
    {

        public BSExpression[] Block;
        public BSExpression Enumerator;
        public string[] Vars;

        public override bool IsConstant => false;

        #region Public

        public BSForeachExpression(
            SourcePosition srcPos,
            string[] vars,
            BSExpression enumExpr,
            BSExpression[] block ) : base( srcPos )
        {
            Vars = vars;
            Enumerator = enumExpr;
            Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSScope foreachScope = new BSScope( BSScopeFlags.Loop, scope );
            ABSObject eObj = Enumerator.Execute( foreachScope ).ResolveReference();

            if ( eObj.HasProperty( "MoveNext" ) && eObj.HasProperty( "GetCurrent" ) )
            {
                ABSObject mnext = eObj.GetProperty( "MoveNext" );
                ABSObject gcurr = eObj.GetProperty( "GetCurrent" );

                Enumerate( scope, foreachScope, mnext, gcurr );
            }
            else if ( eObj is IEnumerable < IForEachIteration > forEach )
            {
                Enumerate( scope, foreachScope, forEach );
            }
            else if ( eObj is IEnumerable < ABSObject > sForEach )
            {
                Enumerate( scope, foreachScope, sForEach );
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

        #region Private

        private void Enumerate( BSScope scope, BSScope foreachScope, IEnumerable < IForEachIteration > forEach )
        {
            foreach ( IForEachIteration iter in forEach )
            {
                ABSObject[] objs = iter.GetObjects();

                for ( int i = 0; i < Vars.Length; i++ )
                {
                    foreachScope.AddLocalVar( Vars[i], objs.Length > i ? objs[i] : BSObject.Null );
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                     foreachScope,
                     Block,
                     Array.Empty < BSFunctionParameter >(),
                     Array.Empty < ABSObject >()
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

        private void Enumerate( BSScope scope, BSScope foreachScope, ABSObject moveNext, ABSObject getCurrent )
        {
            while ( moveNext.Invoke( Array.Empty < ABSObject >() ) != BSObject.False)
            {
                ABSObject[] objs = { getCurrent.Invoke( Array.Empty < ABSObject >() ) };

                for ( int i = 0; i < Vars.Length; i++ )
                {
                    foreachScope.AddLocalVar( Vars[i], objs.Length > i ? objs[i] : BSObject.Null);
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                     foreachScope,
                     Block,
                     Array.Empty < BSFunctionParameter >(),
                     Array.Empty < ABSObject >()
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

        private void Enumerate( BSScope scope, BSScope foreachScope, IEnumerable < ABSObject > forEach )
        {
            foreach ( ABSObject iter in forEach )
            {
                for ( int i = 0; i < Vars.Length; i++ )
                {
                    foreachScope.AddLocalVar( Vars[i], 0 == i ? iter : BSObject.Null );
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                     foreachScope,
                     Block,
                     Array.Empty < BSFunctionParameter >(),
                     Array.Empty < ABSObject >()
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

        #endregion

    }

}

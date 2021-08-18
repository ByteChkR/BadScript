using System.Collections.Generic;
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

                for ( int i = 0; i < m_Vars.Length; i++ )
                {
                    foreachScope.AddLocalVar( m_Vars[i], objs.Length > i ? objs[i] : BSObject.Null );
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    foreachScope,
                    Block,
                    new BSFunctionParameter[0],
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

        private void Enumerate( BSScope scope, BSScope foreachScope, ABSObject moveNext, ABSObject getCurrent )
        {
            while ( moveNext.Invoke( new ABSObject[0] ) != BSObject.Zero )
            {
                ABSObject[] objs = { getCurrent.Invoke( new ABSObject[0] ) };

                for ( int i = 0; i < m_Vars.Length; i++ )
                {
                    foreachScope.AddLocalVar( m_Vars[i], objs.Length > i ? objs[i] : BSObject.Null );
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    foreachScope,
                    Block,
                    new BSFunctionParameter[0],
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

        private void Enumerate( BSScope scope, BSScope foreachScope, IEnumerable < ABSObject > forEach )
        {
            foreach ( ABSObject iter in forEach )
            {

                for ( int i = 0; i < m_Vars.Length; i++ )
                {
                    foreachScope.AddLocalVar( m_Vars[i], 0 == i ? iter : BSObject.Null );
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    foreachScope,
                    Block,
                    new BSFunctionParameter[0],
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

        #endregion
    }

}

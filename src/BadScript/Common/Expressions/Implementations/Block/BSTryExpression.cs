using System;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSTryExpression : BSExpression
    {
        private BSExpression[] m_TryBlock;
        private BSExpression[] m_CatchBlock;
        private string m_CapturedVar;

        #region Public

        public BSTryExpression( BSExpression[] tryBlock, BSExpression[] catchBlock, string capturedVar )
        {
            m_TryBlock = tryBlock;
            m_CatchBlock = catchBlock;
            m_CapturedVar = capturedVar;
        }

        public override ABSObject Execute( BSScope scope )
        {
            try
            {
                BSScope tryScope = new BSScope( BSScopeFlags.None, scope );

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    tryScope,
                    m_TryBlock,
                    new string[0],
                    new ABSObject[0]
                );

                if ( ret != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, ret );
                }
                else if ( tryScope.BreakExecution )
                {
                    scope.SetFlag( tryScope.Flags );
                }

            }
            catch ( Exception e )
            {
                BSScope catchScope = new BSScope( BSScopeFlags.None, scope );

                if ( !string.IsNullOrEmpty( m_CapturedVar ) )
                {
                    catchScope.AddLocalVar( m_CapturedVar, MakeExceptionTable( e ) );
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    catchScope,
                    m_CatchBlock,
                    new string[0],
                    new ABSObject[0]
                );

                if ( ret != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, ret );
                }
                else if ( catchScope.BreakExecution )
                {
                    scope.SetFlag( catchScope.Flags );
                }
            }

            return new BSObject( null );
        }

        #endregion

        #region Private

        private static ABSObject MakeExceptionTable( Exception e )
        {
            if ( e == null )
            {
                return new BSObject( null );
            }

            ABSTable t = new BSTable();
            t.InsertElement( new BSObject( "type" ), new BSObject( e.GetType().Name ) );
            t.InsertElement( new BSObject( "trace" ), new BSObject( e.StackTrace ) );
            t.InsertElement( new BSObject( "message" ), new BSObject( e.Message ) );

            t.InsertElement(
                new BSObject( "getInner" ),
                new BSFunction( "function getInner()", objects => MakeExceptionTable( e.InnerException ), 0, 0 ) );

            return t;
        }

        #endregion
    }

}

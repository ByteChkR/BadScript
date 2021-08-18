using System;
using System.Linq;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSTryExpression : BSExpression
    {
        public BSExpression[] TryBlock;
        public BSExpression[] CatchBlock;
        public readonly string CapturedVar;

        public override bool IsConstant => TryBlock.All( x => x.IsConstant ) && CatchBlock.All( x => x.IsConstant );

        #region Public

        public BSTryExpression(
            SourcePosition srcPos,
            BSExpression[] tryBlock,
            BSExpression[] catchBlock,
            string capturedVar ) : base( srcPos )
        {
            TryBlock = tryBlock;
            CatchBlock = catchBlock;
            CapturedVar = capturedVar;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSFunction stackTop = BSFunction.GetTopStack();

            try
            {
                BSScope tryScope = new BSScope( BSScopeFlags.None, scope );

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    tryScope,
                    TryBlock,
                    new BSFunctionParameter[0],
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
                string trace = BSFunction.FlatTrace;
                BSFunction.RestoreStack( stackTop );
                BSScope catchScope = new BSScope( BSScopeFlags.None, scope );

                if ( !string.IsNullOrEmpty( CapturedVar ) )
                {
                    catchScope.AddLocalVar( CapturedVar, MakeExceptionTable( trace, m_Position, e ) );
                }

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    catchScope,
                    CatchBlock,
                    new BSFunctionParameter[0],
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

            return BSObject.Null;
        }

        #endregion

        #region Private

        private static ABSObject MakeExceptionTable( string stack, SourcePosition p, Exception e )
        {
            if ( e == null )
            {
                return BSObject.Null;
            }

            ABSTable t = new BSTable( p );
            t.InsertElement( new BSObject( "type" ), new BSObject( e.GetType().Name ) );
            t.InsertElement( new BSObject( "cs_trace" ), new BSObject( e.StackTrace ) );

            t.InsertElement( new BSObject( "trace" ), new BSObject( stack ) );
            t.InsertElement( new BSObject( "message" ), new BSObject( e.Message ) );

            t.InsertElement(
                new BSObject( "getInner" ),
                new BSFunction(
                    "function getInner()",
                    objects => MakeExceptionTable( stack, p, e.InnerException ),
                    0,
                    0 ) );

            return t;
        }

        #endregion
    }

}

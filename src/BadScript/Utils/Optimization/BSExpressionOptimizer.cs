using System;
using System.Collections.Generic;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Binary;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Expressions.Implementations.Unary;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Operators.Implementations;
using BadScript.Common.Types;

namespace BadScript.Utils.Optimization
{

    public static class BSExpressionOptimizer
    {
        public static bool WriteLogs = true;

        #region Public

        public static void Optimize( BSExpression[] expr )
        {
            for ( int i = 0; i < expr.Length; i++ )
            {
                expr[i] = OptimizeExpression( expr[i] );
            }
        }

        #endregion

        #region Private

        private static BSExpression OptimizeExpression( BSExpression expr )
        {

            if ( expr is BSFunctionDefinitionExpression fdef )
            {
                for ( int i = 0; i < fdef.Block.Length; i++ )
                {
                    fdef.Block[i] = OptimizeExpression( fdef.Block[i] );
                }
            }

            if ( expr is BSBinaryExpression bin )
            {
                if ( bin.IsConstant )
                {
                    ABSObject o = bin.Execute( null );

                    if ( WriteLogs )
                    {
                        Console.WriteLine(
                            $"[Expression Optimizer] Optimizing {bin.GetType().Name}: " + o.SafeToString() );
                    }

                    return new BSProxyExpression( SourcePosition.Unknown, o );
                }
                else
                {
                    if ( bin.Left.IsConstant )
                    {
                        bin.Left = OptimizeExpression( bin.Left );
                    }

                    if ( bin.Right.IsConstant )
                    {
                        bin.Right = OptimizeExpression( bin.Right );
                    }
                }
            }
            else if ( expr is BSInvocationExpression invoc )
            {
                if ( invoc.IsConstant )
                {
                    ABSObject o = invoc.Execute( null );

                    if ( WriteLogs )
                    {
                        Console.WriteLine(
                            $"[Expression Optimizer] Optimizing {invoc.GetType().Name}: " + o.SafeToString() );
                    }

                    return new BSProxyExpression( SourcePosition.Unknown, o );
                }

                for ( int i = 0; i < invoc.Parameters.Length; i++ )
                {
                    invoc.Parameters[i] = OptimizeExpression( invoc.Parameters[i] );
                }
            }
            else if ( expr is BSUnaryExpression unary )
            {
                if ( unary.IsConstant &&
                     !( unary is BSReturnExpression ) )
                {
                    ABSObject o = unary.Execute( null );

                    if ( WriteLogs )
                    {
                        Console.WriteLine(
                            $"[Expression Optimizer] Optimizing {unary.GetType().Name}: " + o.SafeToString() );
                    }

                    return new BSProxyExpression( SourcePosition.Unknown, o );
                }
                else
                {
                    unary.Left = OptimizeExpression( unary.Left );
                }
            }
            else if ( expr is BSIfExpression ifExpr )
            {
                Dictionary < BSExpression, BSExpression[] > newConditions =
                    new Dictionary < BSExpression, BSExpression[] >();

                foreach ( KeyValuePair < BSExpression, BSExpression[] > keyValuePair in ifExpr.ConditionMap )
                {
                    for ( int i = 0; i < keyValuePair.Value.Length; i++ )
                    {
                        keyValuePair.Value[i] = OptimizeExpression( keyValuePair.Value[i] );
                    }

                    newConditions[OptimizeExpression( keyValuePair.Key )] = keyValuePair.Value;
                }

                ifExpr.ConditionMap = newConditions;

                if ( ifExpr.ElseBlock != null )
                {
                    for ( int i = 0; i < ifExpr.ElseBlock.Length; i++ )
                    {
                        ifExpr.ElseBlock[i] = OptimizeExpression( ifExpr.ElseBlock[i] );
                    }
                }
            }
            else if ( expr is BSTryExpression tryExpr )
            {
                for ( int i = 0; i < tryExpr.TryBlock.Length; i++ )
                {
                    tryExpr.TryBlock[i] = OptimizeExpression( tryExpr.TryBlock[i] );
                }

                for ( int i = 0; i < tryExpr.CatchBlock.Length; i++ )
                {
                    tryExpr.CatchBlock[i] = OptimizeExpression( tryExpr.CatchBlock[i] );
                }
            }
            else if ( expr is BSWhileExpression whileExpr )
            {
                whileExpr.Condition = OptimizeExpression( whileExpr.Condition );

                for ( int i = 0; i < whileExpr.Block.Length; i++ )
                {
                    whileExpr.Block[i] = OptimizeExpression( whileExpr.Block[i] );
                }
            }
            else if ( expr is BSForExpression forExpr )
            {
                forExpr.CounterCondition = OptimizeExpression( forExpr.CounterCondition );
                forExpr.CounterDefinition = OptimizeExpression( forExpr.CounterDefinition );
                forExpr.CounterIncrement = OptimizeExpression( forExpr.CounterIncrement );

                for ( int i = 0; i < forExpr.Block.Length; i++ )
                {
                    forExpr.Block[i] = OptimizeExpression( forExpr.Block[i] );
                }
            }
            else if ( expr is BSForeachExpression foreachExpr )
            {
                foreachExpr.Enumerator = OptimizeExpression( foreachExpr.Enumerator );

                for ( int i = 0; i < foreachExpr.Block.Length; i++ )
                {
                    foreachExpr.Block[i] = OptimizeExpression( foreachExpr.Block[i] );
                }
            }

            return expr;
        }

        #endregion
    }

}

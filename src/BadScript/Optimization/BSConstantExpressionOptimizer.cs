﻿using System;
using System.Collections.Generic;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Binary;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Parser.Expressions.Implementations.Unary;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Parser.Operators.Implementations;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Optimization
{

    public static class BSConstantExpressionOptimizer
    {

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
                    BSExpressionOptimizerMetaData md = null;

                    if ( o is IBSWrappedObject wo )
                    {
                        md = new BSExpressionOptimizerMetaData( wo.GetInternalObject() );
                    }

                    if ( BSEngineSettings.EnableOptimizerWriteLogs )
                    {
                        Console.WriteLine(
                                          $"[Expression Optimizer] Optimizing {bin.GetType().Name}: " + o.SafeToString()
                                         );
                    }

                    return new BSProxyExpression( SourcePosition.Unknown, o, md );
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
                    BSExpressionOptimizerMetaData md = null;

                    if ( o is IBSWrappedObject wo )
                    {
                        md = new BSExpressionOptimizerMetaData( wo.GetInternalObject() );
                    }

                    if ( BSEngineSettings.EnableOptimizerWriteLogs )
                    {
                        Console.WriteLine(
                                          $"[Expression Optimizer] Optimizing {invoc.GetType().Name}: " +
                                          o.SafeToString()
                                         );
                    }

                    return new BSProxyExpression( SourcePosition.Unknown, o, md );
                }

                for ( int i = 0; i < invoc.Parameters.Length; i++ )
                {
                    invoc.Parameters[i] = OptimizeExpression( invoc.Parameters[i] );
                }
            }
            else if ( expr is BSUnaryExpression unary )
            {
                if ( unary.IsConstant )
                {
                    if ( unary is BSReturnExpression r )
                    {
                        r.Left = OptimizeExpression( r.Left );
                    }
                    else
                    {
                        ABSObject o = unary.Execute( null );
                        BSExpressionOptimizerMetaData md = null;

                        if ( o is IBSWrappedObject wo )
                        {
                            md = new BSExpressionOptimizerMetaData( wo.GetInternalObject() );
                        }

                        if ( BSEngineSettings.EnableOptimizerWriteLogs )
                        {
                            Console.WriteLine(
                                              $"[Expression Optimizer] Optimizing {unary.GetType().Name}: " +
                                              o.SafeToString()
                                             );
                        }

                        return new BSProxyExpression( SourcePosition.Unknown, o, md );
                    }
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

                List < BSExpression > remList = new List < BSExpression >();

                foreach ( KeyValuePair < BSExpression, BSExpression[] > newCondition in newConditions )
                {
                    if ( newCondition.Key is BSProxyExpression pexpr )
                    {
                        if ( pexpr.Object.TryConvertBool( out bool c ) && c )
                        {
                            if ( BSEngineSettings.EnableOptimizerWriteLogs )
                            {
                                Console.WriteLine(
                                                  $"[Expression Optimizer] Replacing If Branch with If Block.."
                                                 );
                            }

                            return new BSBlockExpression( newCondition.Value );
                        }
                        else
                        {
                            remList.Add( pexpr );
                        }
                    }
                    else if ( newCondition.Key is BSValueExpression vexpr )
                    {
                        if ( vexpr.SourceValue is true )
                        {
                            if ( BSEngineSettings.EnableOptimizerWriteLogs )
                            {
                                Console.WriteLine(
                                                  $"[Expression Optimizer] Replacing If Branch with If Block.."
                                                 );
                            }

                            return new BSBlockExpression( newCondition.Value );
                        }
                        else
                        {
                            remList.Add( vexpr );
                        }
                    }
                    else if ( newCondition.Value.Length == 0 )
                    {
                        remList.Add( newCondition.Key );
                    }
                }

                foreach ( BSExpression bsExpression in remList )
                {
                    if ( BSEngineSettings.EnableOptimizerWriteLogs )
                    {
                        Console.WriteLine(
                                          $"[Expression Optimizer] Removing If Branch {bsExpression}"
                                         );
                    }

                    newConditions.Remove( bsExpression );
                }

                if ( newConditions.Count == 0 )
                {
                    if ( BSEngineSettings.EnableOptimizerWriteLogs )
                    {
                        Console.WriteLine(
                                          $"[Expression Optimizer] Replacing If Branch with Else Block.."
                                         );
                    }

                    if ( ifExpr.ElseBlock != null && ifExpr.ElseBlock.Length != 0 )
                    {
                        return new BSBlockExpression( ifExpr.ElseBlock );
                    }

                    return new BSProxyExpression(
                                                 SourcePosition.Unknown,
                                                 BSObject.Null,
                                                 new BSExpressionOptimizerMetaData( null )
                                                );
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

                if ( whileExpr.Condition is BSProxyExpression pexpr && pexpr.Object.TryConvertBool( out bool v ) && !v )
                {
                    if ( BSEngineSettings.EnableOptimizerWriteLogs )
                    {
                        Console.WriteLine(
                                          $"[Expression Optimizer] Removing Unreachable While Loop"
                                         );
                    }

                    return new BSProxyExpression(
                                                 SourcePosition.Unknown,
                                                 BSObject.Null,
                                                 new BSExpressionOptimizerMetaData( null )
                                                );
                }

                if ( whileExpr.Condition is BSValueExpression vexpr && vexpr.SourceValue is false )
                {
                    if ( BSEngineSettings.EnableOptimizerWriteLogs )
                    {
                        Console.WriteLine(
                                          $"[Expression Optimizer] Removing Unreachable While Loop"
                                         );
                    }

                    return new BSProxyExpression(
                                                 SourcePosition.Unknown,
                                                 BSObject.Null,
                                                 new BSExpressionOptimizerMetaData( null )
                                                );
                }

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

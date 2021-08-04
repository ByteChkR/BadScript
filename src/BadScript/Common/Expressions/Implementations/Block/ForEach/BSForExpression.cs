﻿using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Block.ForEach
{

    public class BSForExpression : BSExpression
    {
        public BSExpression CounterDefinition;
        public BSExpression CounterCondition;
        public BSExpression CounterIncrement;
        public BSExpression[] Block;
        public override bool IsConstant => CounterDefinition.IsConstant &&
                                           CounterCondition.IsConstant &&
                                           CounterIncrement.IsConstant;

        #region Public

        public BSForExpression(
            SourcePosition srcPos,
            BSExpression cDef,
            BSExpression cCond,
            BSExpression cInc,
            BSExpression[] block ) : base( srcPos )
        {
            CounterCondition = cCond;
            CounterDefinition = cDef;
            CounterIncrement = cInc;
            Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSScope forScope = new BSScope( BSScopeFlags.Loop, scope );
            CounterDefinition.Execute( forScope );
            ABSObject c = CounterCondition.Execute( forScope ).ResolveReference();

            while ( c.TryConvertBool( out bool d ) && d )
            {

                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                    forScope,
                    Block,
                    new string[0],
                    new ABSObject[0]
                );

                if ( forScope.Flags == BSScopeFlags.Continue )
                {
                    scope.SetFlag( BSScopeFlags.None );
                }

                if ( ret != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, ret );

                    break;
                }
                else if ( forScope.Flags == BSScopeFlags.Break )
                {
                    break;
                }

                CounterIncrement.Execute( forScope );

                c = CounterCondition.Execute( forScope ).ResolveReference();

            }

            return BSObject.Null;
        }

        #endregion
    }

}

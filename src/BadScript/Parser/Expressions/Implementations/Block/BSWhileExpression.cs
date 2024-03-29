﻿using System;

using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.Expressions.Implementations.Block
{

    public class BSWhileExpression : BSExpression
    {

        public BSExpression Condition;
        public BSExpression[] Block;

        public override bool IsConstant => Condition.IsConstant;

        #region Public

        public BSWhileExpression( SourcePosition srcPos, BSExpression condition, BSExpression[] block ) : base( srcPos )
        {
            Condition = condition;
            Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSScope funcScope = new BSScope( BSScopeFlags.Loop, scope );
            ABSObject o = Condition.Execute( funcScope ).ResolveReference();

            while ( o.TryConvertBool( out bool d ) && d )
            {
                ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                     funcScope,
                     Block,
                     Array.Empty < BSFunctionParameter >(),
                     Array.Empty < ABSObject >()
                    );

                if ( funcScope.Flags == BSScopeFlags.Continue )
                {
                    funcScope.SetFlag( BSScopeFlags.None );
                }

                if ( ret != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, ret );

                    break;
                }
                else if ( funcScope.Flags == BSScopeFlags.Break )
                {
                    break;
                }

                o = Condition.Execute( funcScope ).ResolveReference();
            }

            return BSObject.Null;
        }

        #endregion

    }

}

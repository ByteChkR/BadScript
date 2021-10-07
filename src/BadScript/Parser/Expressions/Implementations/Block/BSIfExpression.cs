using System;
using System.Collections.Generic;
using System.Linq;

using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Parser.Expressions.Implementations.Block
{

    public class BSIfExpression : BSExpression
    {

        public Dictionary < BSExpression, BSExpression[] > ConditionMap;
        public BSExpression[] ElseBlock;

        public override bool IsConstant => ConditionMap.All( x => x.Key.IsConstant );

        #region Public

        public BSIfExpression(
            SourcePosition srcPos,
            Dictionary < BSExpression, BSExpression[] > conditions,
            BSExpression[] elseBlock = null ) : base( srcPos )
        {
            ConditionMap = conditions;
            ElseBlock = elseBlock;
        }

        public override ABSObject Execute( BSScope scope )
        {
            foreach ( KeyValuePair < BSExpression, BSExpression[] > keyValuePair in ConditionMap )
            {
                ABSObject o = keyValuePair.Key.Execute( scope ).ResolveReference();
                bool d = o.ConvertBool();

                if ( d )
                {
                    BSScope funcScope = new BSScope( BSScopeFlags.IfBlock, scope );

                    ABSObject ret = BSFunctionDefinitionExpression.InvokeBlockFunction(
                         funcScope,
                         keyValuePair.Value,
                         Array.Empty < BSFunctionParameter >(),
                         Array.Empty < ABSObject >()
                        );

                    if ( ret != null )
                    {
                        scope.SetFlag( BSScopeFlags.Return, ret );
                    }
                    else if ( funcScope.BreakExecution )
                    {
                        scope.SetFlag( funcScope.Flags );
                    }

                    return BSObject.Null;
                }
            }

            if ( ElseBlock != null )
            {
                BSScope elseScope = new BSScope( BSScopeFlags.IfBlock, scope );

                ABSObject elseR = BSFunctionDefinitionExpression.InvokeBlockFunction(
                     elseScope,
                     ElseBlock,
                     Array.Empty < BSFunctionParameter >(),
                     Array.Empty < ABSObject >()
                    );

                if ( elseR != null )
                {
                    scope.SetFlag( BSScopeFlags.Return, elseR );
                }
                else
                {
                    scope.SetFlag( elseScope.Flags );
                }
            }

            return BSObject.Null;
        }

        #endregion

    }

}

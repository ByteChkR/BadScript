﻿using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSBreakExpression : BSExpression
    {
        #region Public

        public BSBreakExpression()
        {
        }

        public override ABSObject Execute( BSScope scope )
        {
            scope.SetFlag( BSScopeFlags.Break );

            return new BSObject( null );
        }

        #endregion
    }

}

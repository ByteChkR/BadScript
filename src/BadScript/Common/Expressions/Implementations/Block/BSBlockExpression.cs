﻿using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSBlockExpression : BSExpression
    {

        public BSExpression[] Block;

        public override bool IsConstant => false;

        #region Public

        public BSBlockExpression( BSExpression[] block ) : base( SourcePosition.Unknown )
        {
            Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            foreach ( BSExpression bsExpression in Block )
            {
                bsExpression.Execute( scope );

                if ( scope.BreakExecution )
                {
                    break;
                }
            }

            return BSObject.Null;
        }

        #endregion

    }

}
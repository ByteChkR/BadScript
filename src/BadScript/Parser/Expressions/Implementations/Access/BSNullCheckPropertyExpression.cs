﻿using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Access
{

    public class BSNullCheckPropertyExpression : BSExpression
    {

        public readonly string Right;

        public override bool IsConstant => false;

        public BSExpression Left { get; private set; }

        #region Public

        public BSNullCheckPropertyExpression( SourcePosition srcPos, BSExpression left, string varname ) : base(
             srcPos
            )
        {
            Left = left;
            Right = varname;
        }

        public override ABSObject Execute( BSScope scope )
        {
            if ( Left != null )
            {
                ABSObject l = Left.Execute( scope );

                if ( !l.HasProperty( Right ) )
                {
                    return BSObject.Null;
                }

                return Left.Execute( scope ).GetProperty( Right );
            }

            return scope.ResolveName( Right );
        }

        #endregion

    }

}

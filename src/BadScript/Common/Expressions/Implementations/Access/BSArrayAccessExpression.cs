using System;
using BadScript.Common.Expressions.Implementations.Unary;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Access
{

    public class BSArrayAccessExpression : BSUnaryExpression
    {
        private BSExpression Parameter;

        #region Public

        public BSArrayAccessExpression( BSExpression left, BSExpression arg ) : base( left )
        {
            Parameter = arg;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject obj = Left.Execute( scope ).ResolveReference();

            ABSObject i = Parameter.Execute( scope ).ResolveReference();

            if ( obj is ABSTable t )
            {
                return t.GetElement( i );
            }

            if ( obj is ABSArray a )
            {
                if ( i.TryConvertDecimal( out decimal d ) )
                {
                    return a.GetElement( ( int ) d );
                }
            }

            throw new Exception( $"Can not access {obj} as array" );
        }

        #endregion
    }

}

using System;

using BadScript.Parser.Expressions.Unary;
using BadScript.Runtime;

namespace BadScript.Parser.Expressions.Value
{

    public class BSArrayAccessExpression : BSUnaryExpression
    {

        private BSExpression Parameter;
        #region Public

        public BSArrayAccessExpression( BSExpression left, BSExpression arg ) : base( left )
        {
            Parameter = arg;
        }

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            BSRuntimeObject obj = Left.Execute( scope );

            if ( obj is BSRuntimeReference r )
            {
                obj = r.Get();
            }

            BSRuntimeObject i = Parameter.Execute( scope );

            if ( i is BSRuntimeReference ir )
            {
                i = ir.Get();
            }

            if ( obj is BSRuntimeTable t )
            {
                return t.GetElement( i );
            }

            if ( obj is BSRuntimeArray a )
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

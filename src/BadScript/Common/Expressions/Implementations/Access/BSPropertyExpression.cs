using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Access
{

    public class BSPropertyExpression : BSExpression
    {
        public readonly string Right;

        public override bool IsConstant => false;

        public BSExpression Left { get; private set; }

        #region Public

        public BSPropertyExpression( SourcePosition srcPos, BSExpression left, string varname ) : base( srcPos )
        {
            Left = left;
            Right = varname;
        }

        public override ABSObject Execute( BSScope scope )
        {
            if ( Left != null )
            {
                ABSObject l = Left.Execute( scope );

                if(BSOperatorImplementationResolver.AllowOperatorOverrides)
                {
                    string pStr = BSOperatorImplementationResolver.ResolveKey( "." );

                    if ( l.HasProperty( pStr ) )
                    {
                        return l.GetProperty( pStr ).Invoke( new ABSObject[] { new BSObject( Right ) } );
                    }
                }

                return l.GetProperty( Right );
            }

            return scope.ResolveName( Right );
        }

        #endregion
    }

}

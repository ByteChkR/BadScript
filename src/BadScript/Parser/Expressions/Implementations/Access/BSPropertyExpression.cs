using BadScript.Parser.OperatorImplementations;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.Implementations.Types;

namespace BadScript.Parser.Expressions.Implementations.Access
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

                if ( BSEngineSettings.EnableCoreFastTrack )
                {
                    if ( l is BSArray ||
                         l is BSTable ||
                         l is BSClassInstance ||
                         l is BSFunction )
                    {
                        return l.GetProperty( Right );
                    }
                }

                ABSOperatorImplementation impl = BSOperatorImplementationResolver.ResolveImplementation(
                     ".",
                     new[] { l, new BSObject( Right ) },
                     true
                    );

                return impl.ExecuteOperator( new[] { l, new BSObject( Right ) } );
            }

            return scope.ResolveName( Right );
        }

        #endregion

    }

}

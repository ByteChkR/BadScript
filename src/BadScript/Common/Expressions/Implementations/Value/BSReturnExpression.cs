using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.References;

namespace BadScript.Common.Expressions.Implementations.Value
{

    public class BSReturnExpression : BSExpression
    {
        public BSExpression Left { get; private set; }

        #region Public

        public BSReturnExpression( SourcePosition srcPos, BSExpression left ) : base( srcPos )
        {
            Left = left;
        }

        public override ABSObject Execute( BSScope scope )
        {
            ABSObject o = Left.Execute( scope ).ResolveReference();

            scope.SetFlag( BSScopeFlags.Return, o );

            return o;
        }

        #endregion
    }

}

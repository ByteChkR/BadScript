using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Types
{

    public class BSUsingExpression : BSExpression
    {

        public readonly string[] FullName;

        public override bool IsConstant => false;

        #region Public

        public BSUsingExpression( SourcePosition pos, string[] fullName ) : base( pos )
        {
            FullName = fullName;
        }

        public override ABSObject Execute( BSScope scope )
        {
            scope.Namespace.AddUsing( scope.Engine.NamespaceRoot.GetNamespace( FullName ) );

            return BSObject.Null;
        }

        #endregion

    }

}

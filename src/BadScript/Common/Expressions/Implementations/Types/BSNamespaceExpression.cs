using BadScript.Common.Namespaces;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Types
{

    public class BSNamespaceExpression : BSExpression
    {

        public readonly string[] FullName;

        public readonly BSExpression[] Block;

        public override bool IsConstant => false;

        #region Public

        public BSNamespaceExpression( SourcePosition pos, string[] fullName, BSExpression[] block ) : base( pos )
        {
            FullName = fullName;
            Block = block;
        }

        public override ABSObject Execute( BSScope scope )
        {
            BSNamespace ns = CreateSelf( scope.Engine.NamespaceRoot );

            BSScope nsScope = new BSScope( BSScopeFlags.None, scope );
            nsScope.SetNamespace( ns );

            foreach ( BSExpression bsExpression in Block )
            {
                bsExpression.Execute( nsScope );
            }

            return BSObject.Null;
        }

        #endregion

        #region Private

        private BSNamespace CreateSelf( BSNamespace root )
        {
            BSNamespace current = root;

            foreach ( string s in FullName )
            {
                current = current.GetOrCreateNamespace( s );
            }

            return current;
        }

        #endregion

    }

}

using BadScript.Common.Expressions.Implementations.Types;

namespace BadScript.Common.Namespaces
{

    public class BSNamespaceRoot : BSNamespace
    {

        #region Public

        public BSNamespaceRoot() : base( null, "" )
        {
        }

        public override bool ContainsType( string name, bool includeChildren )
        {
            return HasType( name, includeChildren );
        }

        public BSNamespace GetNamespace( string[] fullName )
        {
            BSNamespace current = this;

            foreach ( string s in fullName )
            {
                current = current.GetNamespace( s );
            }

            return current;
        }

        public override BSClassExpression ResolveType( string name, bool includeChildren )
        {
            return GetType( name, includeChildren );
        }

        #endregion

    }

}

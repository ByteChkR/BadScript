using BadScript.Common.Expressions.Implementations.Types;
using BadScript.Common.Expressions.Implementations.Value;

namespace BadScript.Common.Namespaces
{

    public class BSNamespaceRoot: BSNamespace
    {

        public BSNamespaceRoot() : base(null, "") { }

        public override bool ContainsType( string name, bool includeChildren) => HasType( name, includeChildren);

        public override BSClassExpression ResolveType( string name, bool includeChildren) => GetType( name, includeChildren );

        public BSNamespace GetNamespace( string[] fullName )
        {
            BSNamespace current = this;

            foreach (string s in fullName)
            {
                current = current.GetNamespace(s);
            }

            return current;
        }

    }

}
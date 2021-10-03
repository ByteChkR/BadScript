using BadScript.Namespaces;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Types;
using BadScript.Scopes;
using BadScript.Types.References;

namespace BadScript.Types.Implementations.Types
{

    public static class BSTypeDatabase
    {

        #region Public

        public static BSClassInstance CreateInstance(
            string name,
            BSEngine engine,
            BSNamespace start,
            ABSObject[] args )
        {
            BSScope classScope = new BSScope( engine );

            return CreateBaseInstance( name, classScope, start, args );
        }

        #endregion

        #region Private

        private static BSClassInstance CreateBaseInstance(
            string name,
            BSScope scope,
            BSNamespace start,
            ABSObject[] args )
        {
            BSClassExpression expr = start.ResolveType( name, true );
            BSClassInstance baseInstance = null;

            if ( expr.BaseName != null )
            {
                baseInstance = CreateBaseInstance( expr.BaseName, scope, expr.DefiningScope.Namespace, null );
                scope = new BSScope( BSScopeFlags.None, baseInstance.InstanceScope );
            }

            expr.AddClassData( scope );

            BSClassInstance table = new BSClassInstance( SourcePosition.Unknown, name, baseInstance, scope );

            if ( args != null )
            {
                if ( table.HasProperty( table.Name ) )
                {
                    ABSObject func = table.GetProperty( table.Name ).ResolveReference();
                    func.Invoke( args );
                }
            }

            return table;
        }

        #endregion

    }

}

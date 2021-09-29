using System.Collections.Generic;
using System.Linq;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Types;

namespace BadScript.Common.Namespaces
{

    public class BSNamespace
    {

        public readonly BSNamespace Parent;
        public readonly string Name;

        private readonly List < BSNamespace > m_Children = new();
        private readonly List < BSNamespace > m_IncludedNamespaces = new();
        private readonly List < BSClassExpression > m_Types = new();

        public string FullName => string.IsNullOrEmpty( Parent.Name ) ? Name : Parent.FullName + "." + Name;

        #region Public

        public BSNamespace( BSNamespace parent, string name )
        {
            Name = name;
            Parent = parent;
        }

        public void AddType( BSClassExpression expr )
        {
            m_Types.Add( expr );
        }

        public void AddUsing( BSNamespace ns )
        {
            m_IncludedNamespaces.Add( ns );
        }

        public virtual bool ContainsType( string name, bool includeChildren )
        {
            return HasType( name, includeChildren ) || Parent.ContainsType( name, false );
        }

        public BSNamespace GetNamespace( string name )
        {
            return m_Children.First( x => x.Name == name );
        }

        public BSNamespace GetOrCreateNamespace( string name )
        {
            BSNamespace ns = m_Children.FirstOrDefault( x => x.Name == name );

            if ( ns == null )
            {
                ns = new BSNamespace( this, name );
                m_Children.Add( ns );
            }

            return ns;
        }

        public virtual BSClassExpression ResolveType( string name, bool includeChildren )
        {
            if (
                HasType( name, includeChildren ) )
            {
                return GetType( name, includeChildren );
            }
            else if ( Parent.ContainsType( name, false ) )
            {
                Parent.ResolveType( name, false );
            }

            throw new BSRuntimeException(
                                         $"Can not find Class '{name}' in namespace '{FullName}' are you missing a using statement?"
                                        );
        }

        public override string ToString()
        {
            return $"namespace {FullName}";
        }

        #endregion

        #region Protected

        protected BSClassExpression GetType( string name, bool includeChildren )
        {
            if ( m_Types.Any( x => x.Name == name ) )
            {
                return m_Types.First( x => x.Name == name );
            }

            if ( m_IncludedNamespaces.Any( x => x.HasType( name, false ) ) )
            {
                foreach ( BSNamespace ns in m_IncludedNamespaces )
                {
                    if ( ns.HasType( name, false ) )
                    {
                        return ns.GetType( name, false );
                    }
                }
            }

            if ( includeChildren && m_Children.Any( x => x.HasType( name, true ) ) )
            {
                foreach ( BSNamespace ns in m_Children )
                {
                    if ( ns.HasType( name, true ) )
                    {
                        return ns.GetType( name, true );
                    }
                }
            }

            throw new BSRuntimeException( $"Can not find Class '{name}' in namespace '{FullName}'" );
        }

        protected bool HasType( string name, bool includeChildren )
        {
            return m_Types.Any( x => x.Name == name ) ||
                   m_IncludedNamespaces.Any( x => x.HasType( name, false ) ) ||
                   includeChildren && m_Children.Any( x => x.HasType( name, true ) );
        }

        #endregion

    }

}

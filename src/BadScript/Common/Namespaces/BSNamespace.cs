using System.Collections.Generic;
using System.Linq;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Types;

namespace BadScript.Common.Namespaces
{
    /// <summary>
    /// Namespace Object used in the Type System
    /// Contains sub namespaces and types.
    /// </summary>
    public class BSNamespace
    {

        /// <summary>
        /// The parent namespace(null if root)
        /// </summary>
        public readonly BSNamespace Parent;
        /// <summary>
        /// The Name of the namespace("" if root)
        /// </summary>
        public readonly string Name;

        private readonly List < BSNamespace > m_Children = new();
        private readonly List < BSNamespace > m_IncludedNamespaces = new();
        private readonly List < BSClassExpression > m_Types = new();

        /// <summary>
        /// The Full Name of the Namespace
        /// Namespaces are seperated by '.'
        /// Example:    MyApp.Internal.Configs
        /// </summary>
        public string FullName => string.IsNullOrEmpty( Parent.Name ) ? Name : Parent.FullName + "." + Name;

        #region Public

        /// <summary>
        /// Creates a new Namespace in the selected parent object.
        /// </summary>
        /// <param name="parent">Parent Namespace</param>
        /// <param name="name">Name of this namespace</param>
        public BSNamespace( BSNamespace parent, string name )
        {
            Name = name;
            Parent = parent;
        }

        /// <summary>
        /// Adds a Type Definition to the namespace
        /// </summary>
        /// <param name="expr"></param>
        public void AddType( BSClassExpression expr )
        {
            m_Types.Add( expr );
        }

        /// <summary>
        /// Adds a used namespace.
        /// All types within that namespace will become accessible to this namespace.
        /// </summary>
        /// <param name="ns">The used namespace</param>
        public void AddUsing( BSNamespace ns )
        {
            m_IncludedNamespaces.Add( ns );
        }

        /// <summary>
        /// Returns true if the namespace contains a type with the specified name. Or if a Parent Namespace contains the type with the specified name
        /// </summary>
        /// <param name="name">Name of the Type</param>
        /// <param name="includeChildren">Should the namespace also include its child namespaces in the search</param>
        /// <returns>True if Contains</returns>
        public virtual bool ContainsType( string name, bool includeChildren )
        {
            return HasType( name, includeChildren ) || Parent.ContainsType( name, false );
        }

        /// <summary>
        /// Returns a Direct Child Namespace by the specified name
        /// </summary>
        /// <param name="name">Name of the Namespace</param>
        /// <returns>Namespace instance</returns>
        public BSNamespace GetNamespace( string name )
        {
            return m_Children.First( x => x.Name == name );
        }

        /// <summary>
        /// Finds or creates a namespace with the specified name
        /// </summary>
        /// <param name="name">Namespace name</param>
        /// <returns></returns>
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

        /// <summary>
        /// Resolves a type by its specified name
        /// </summary>
        /// <param name="name">Name of the Type</param>
        /// <param name="includeChildren">Should the namespace also include its child namespaces in the search</param>
        /// <returns>BS Type Definition</returns>
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

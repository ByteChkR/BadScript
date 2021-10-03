using System.Collections;
using System.Collections.Generic;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Namespaces;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Common.Runtime
{
    /// <summary>
    /// Defines a Scope that scripts can use to store and load elements.
    /// </summary>
    public class BSScope : IEnumerable < IForEachIteration >
    {

        private BSNamespace m_Namespace;

        private readonly BSScopeFlags m_AllowedFlags;
        private BSScopeFlags m_CurrentFlag;
        private readonly BSEngine m_Instance;
        private readonly BSScope m_Parent;
        private readonly BSTable m_LocalVars = new BSTable( SourcePosition.Unknown );

        /// <summary>
        /// The Namespace of this scope
        /// </summary>
        public BSNamespace Namespace => m_Namespace ?? m_Parent.Namespace;

        /// <summary>
        /// Execution flags that indicate what special "execution-flow-altering" flags were set.
        /// </summary>
        public BSScopeFlags Flags => m_CurrentFlag;

        /// <summary>
        /// The Return value of the scope if it returned a value in its execution
        /// </summary>
        public ABSObject Return { get; private set; }

        /// <summary>
        /// Returns true if any of
        ///     BSScopeFlags.Return or
        ///     BSScopeFlags.Break or
        ///     BSScopeFlags.Continue
        /// are set.
        /// </summary>
        public bool BreakExecution =>
            ( m_CurrentFlag & ( BSScopeFlags.Return | BSScopeFlags.Break | BSScopeFlags.Continue ) ) != 0;

        /// <summary>
        /// The Engine Instance that is the root of this scope.
        /// </summary>
        public BSEngine Engine => m_Instance ?? m_Parent.Engine;

        #region Public

        /// <summary>
        /// Creates a new Scope that has no parent.
        /// </summary>
        /// <param name="instance"></param>
        public BSScope( BSEngine instance ) : this()
        {
            m_AllowedFlags = BSScopeFlags.Return;
            m_Instance = instance;
            m_Namespace = m_Instance.NamespaceRoot;
        }

        /// <summary>
        /// Creates a new Scope with a parent and flags that are allowed to be set in this scope
        /// </summary>
        /// <param name="allowedFlags">The Allowed Flags</param>
        /// <param name="parent">The scope that this scope is a child of</param>
        public BSScope( BSScopeFlags allowedFlags, BSScope parent ) : this()
        {
            m_AllowedFlags = allowedFlags | parent.m_AllowedFlags;
            m_Parent = parent;
        }

        /// <summary>
        /// Adds a Variable to the Root Scope(that has no parent)
        /// </summary>
        /// <param name="name">Name of the Variable</param>
        /// <param name="o">Value of the Variable</param>
        public void AddGlobalVar( string name, ABSObject o )
        {
            if ( m_Parent != null )
            {
                m_Parent.AddGlobalVar( name, o );
            }
            else
            {
                m_Instance.InsertElement( new BSObject( name ), o );
            }
        }

        /// <summary>
        /// Adds a Variable to the this scope
        /// </summary>
        /// <param name="name">Name of the Variable</param>
        /// <param name="o">Value of the Variable</param>
        public void AddLocalVar( string name, ABSObject o )
        {
            m_LocalVars.InsertElement( new BSObject( name ), o );
        }

        /// <summary>
        /// Returns the first encountered variable with this name
        /// </summary>
        /// <param name="name">Name of the Variable</param>
        /// <returns>A reference to that variable</returns>
        public ABSReference Get( string name )
        {
            if ( m_LocalVars.HasElement( new BSObject( name ) ) )
            {
                return m_LocalVars.GetProperty( name );
            }

            if ( m_Parent != null && m_Parent.HasLocal( name ) )
            {
                return m_Parent.Get( name );
            }

            throw new BSRuntimeException( "Can not Set Property: " + name );
        }

        /// <summary>
        /// Returns the first encountered variable with this name as readonly reference
        /// </summary>
        /// <param name="name">Name of the Variable</param>
        /// <param name="readonlyRef">if true the returned reference is readonly</param>
        /// <returns>A reference to that variable</returns>
        public ABSReference Get( string name, bool readonlyRef )
        {
            if ( m_LocalVars.HasElement( new BSObject( name ) ) )
            {
                return m_LocalVars.GetProperty( name, readonlyRef );
            }

            if ( m_Parent != null && m_Parent.HasLocal( name ) )
            {
                return m_Parent.Get( name, readonlyRef );
            }

            throw new BSRuntimeException( "Can not Set Property: " + name );
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            return m_LocalVars.GetEnumerator();
        }

        /// <summary>
        /// Get Local Scope Table for this scope
        /// </summary>
        /// <returns></returns>
        public ABSTable GetLocals()
        {
            return m_LocalVars;
        }

        /// <summary>
        /// returns true if the name is contained in this, any parent scope or in the global scope
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Has( string name ) => HasLocal( name ) || HasGlobal( name );

        /// <summary>
        /// Returns true if a parent or the global scope contains a variable with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasGlobal( string name )
        {
            return m_Instance == null
                       ? m_Parent.HasGlobal( name )
                       : m_Instance.HasElement( new BSObject( name ) );
        }
        /// <summary>
        /// Returns true if the local parent or local table contains a variable with this name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasLocal( string name )
        {
            return m_LocalVars.HasElement( new BSObject( name ) ) ||
                   m_Parent != null && m_Parent.HasLocal( name );
        }

        /// <summary>
        /// Tries to resolve the variable with the specified name by looking in all visible scopes
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ABSObject ResolveName( string name )
        {
            ABSObject i = new BSObject( name );

            if ( m_LocalVars.HasElement( i ) )
            {
                return m_LocalVars.GetElement( i );
            }

            if ( m_Parent != null && ( m_Parent.HasLocal( name ) || m_Parent.HasGlobal( name ) ) )
            {
                return m_Parent.ResolveName( name );
            }

            if ( m_Instance != null && m_Instance.HasElement( i ) )
            {
                return m_Instance.GetElement( i );
            }

            return new BSTableReference( m_LocalVars, i, false );
        }

        /// <summary>
        /// Assigns a value to the first variable found with this name
        /// </summary>
        /// <param name="name">Variable Name</param>
        /// <param name="o">Value</param>
        public void Set( string name, ABSObject o )
        {
            if ( HasLocal( name ) )
            {
                m_LocalVars.SetProperty( name, o );
            }
            else if ( HasGlobal( name ) )
            {
                if ( m_Parent != null )
                {
                    m_Parent.Set( name, o );
                }
                else
                {
                    throw new BSRuntimeException( "Can not Set Property: " + name );
                }
            }
            else
            {
                throw new BSRuntimeException( "Can not Set Property: " + name );
            }
        }

        /// <summary>
        /// Resets all BSScopeFlags to BSScopeFlags.None and clears the result property
        /// </summary>
        public void ResetFlag()
        {
            m_CurrentFlag = BSScopeFlags.None;
            Return = null;
        }

        /// <summary>
        /// Sets one or more flags.
        /// Optional can specify a return value
        /// </summary>
        /// <param name="flag">Flags to be set</param>
        /// <param name="val">Optional Return value to be set</param>
        public void SetFlag( BSScopeFlags flag, ABSObject val = null )
        {
            if ( val != null )
            {
                if ( flag != BSScopeFlags.Return )
                {
                    throw new BSRuntimeException(
                                                 "Invalid Use of Return in a scope that does not allow return values"
                                                );
                }

                Return = val;
            }
            else
            {
                if ( flag != BSScopeFlags.None && ( m_AllowedFlags & flag ) == 0 )
                {
                    throw new BSRuntimeException( "Invalid Scope Flag: " + flag );
                }
            }

            m_CurrentFlag = flag;
        }

        /// <summary>
        /// Sets the Current Namespace of the scope
        /// </summary>
        /// <param name="ns"></param>
        public void SetNamespace( BSNamespace ns )
        {
            m_Namespace = ns;
        }

        #endregion

        #region Private

        private BSScope()
        {
            m_LocalVars.InsertElement( new BSObject( "__SELF" ), new BSObject( this ) );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable )m_LocalVars ).GetEnumerator();
        }

        #endregion

    }

}

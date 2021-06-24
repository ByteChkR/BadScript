using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Common.Runtime
{

    public class BSScope
    {
        private readonly BSEngineInstance m_Instance;
        private readonly BSScope m_Parent;
        private readonly BSTable m_LocalVars = new BSTable();

        public ABSObject ReturnValue { get; private set; }

        #region Public

        public BSScope( BSEngineInstance instance ) : this()
        {
            m_Instance = instance;
        }

        public BSScope( BSScope parent ) : this()
        {
            m_Parent = parent;
        }

        public void AddGlobalVar( string name, ABSObject o )
        {
            if ( m_Parent != null )
            {
                m_Parent.AddGlobalVar( name, o );
            }
            else
            {
                m_Instance.GlobalTable.InsertElement( new BSObject( name ), o );
            }
        }

        public void AddLocalVar( string name, ABSObject o )
        {
            m_LocalVars.InsertElement( new BSObject( name ), o );
        }

        public ABSTable GetLocals()
        {
            return m_LocalVars;
        }

        public bool HasGlobal( string name )
        {
            return m_Instance == null
                ? m_Parent.HasGlobal( name )
                : m_Instance.GlobalTable.HasElement( new BSObject( name ) ) ||
                  m_Parent != null && m_Parent.HasLocal( name );
        }

        public bool HasLocal( string name )
        {
            return m_LocalVars.HasElement( new BSObject( name ) ) ||
                   m_Parent != null && m_Parent.HasLocal( name );
        }

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

            if ( m_Instance != null && m_Instance.GlobalTable.HasElement( i ) )
            {
                return m_Instance.GlobalTable.GetElement( i );
            }

            return new BSTableReference( m_LocalVars, i );
        }

        public void SetReturnValue( ABSObject o )
        {
            ReturnValue = o;
        }

        #endregion

        #region Private

        private BSScope()
        {
            m_LocalVars.InsertElement( new BSObject( "__L" ), m_LocalVars );
        }

        #endregion
    }

}

using BadScript.Runtime.Implementations;

namespace BadScript.Runtime
{

    public class BSEngineScope
    {

        private readonly BSEngineInstance m_Instance;
        private readonly BSEngineScope m_Parent;
        private readonly EngineRuntimeTable m_LocalVars = new EngineRuntimeTable();

        public BSRuntimeObject ReturnValue { get; private set; }

        #region Public

        public BSEngineScope( BSEngineInstance instance ) : this()
        {
            m_Instance = instance;
        }

        public BSEngineScope( BSEngineScope parent ) : this()
        {
            m_Parent = parent;
        }

        public void AddGlobalVar( string name, BSRuntimeObject o )
        {
            if ( m_Parent != null )
            {
                m_Parent.AddGlobalVar( name, o );
            }
            else
            {
                m_Instance.GlobalTable.InsertElement( new EngineRuntimeObject( name ), o );
            }
        }

        public void AddLocalVar( string name, BSRuntimeObject o )
        {
            m_LocalVars.InsertElement( new EngineRuntimeObject( name ), o );
        }

        public BSRuntimeTable GetLocals()
        {
            return m_LocalVars;
        }

        public bool HasGlobal( string name )
        {
            return m_Instance == null
                       ? m_Parent.HasGlobal( name )
                       : m_Instance.GlobalTable.HasElement( new EngineRuntimeObject( name ) ) ||
                         m_Parent != null && m_Parent.HasLocal( name );
        }

        public bool HasLocal( string name )
        {
            return m_LocalVars.HasElement( new EngineRuntimeObject( name ) ) ||
                   m_Parent != null && m_Parent.HasLocal( name );
        }

        public BSRuntimeObject ResolveName( string name )
        {
            BSRuntimeObject i = new EngineRuntimeObject( name );

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

            return new BSRuntimeTableReference( m_LocalVars, i );
        }

        public void SetReturnValue( BSRuntimeObject o )
        {
            ReturnValue = o;
        }

        #endregion

        #region Private

        private BSEngineScope()
        {
            m_LocalVars.InsertElement( new EngineRuntimeObject( "__L" ), m_LocalVars );
        }

        #endregion

    }

}

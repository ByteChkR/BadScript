using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Common.Runtime
{

    public class BSScope
    {
        private readonly BSScopeFlags m_AllowedFlags;
        private BSScopeFlags m_CurrentFlag;
        private readonly BSEngineInstance m_Instance;
        private readonly BSScope m_Parent;
        private readonly BSTable m_LocalVars = new BSTable( SourcePosition.Unknown );

        public BSScopeFlags Flags => m_CurrentFlag;

        public ABSObject Return { get; private set; }

        public bool BreakExecution =>
            ( m_CurrentFlag & ( BSScopeFlags.Return | BSScopeFlags.Break | BSScopeFlags.Continue ) ) != 0;

        #region Public

        public BSScope( BSEngineInstance instance ) : this()
        {
            m_AllowedFlags = BSScopeFlags.Return;
            m_Instance = instance;
        }

        public BSScope( BSScopeFlags allowedFlags, BSScope parent ) : this()
        {
            m_AllowedFlags = allowedFlags | parent.m_AllowedFlags;
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
                m_Instance.InsertElement( new BSObject( name ), o );
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
                : m_Instance.HasElement( new BSObject( name ) );
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

            if ( m_Instance != null && m_Instance.HasElement( i ) )
            {
                return m_Instance.GetElement( i );
            }

            return new BSTableReference( m_LocalVars, i, false );
        }

        public void SetFlag( BSScopeFlags flag, ABSObject val = null )
        {
            if ( val != null )
            {
                if ( flag != BSScopeFlags.Return )
                {
                    throw new BSRuntimeException(
                        "Invalid Use of Return in a scope that does not allow return values" );
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

        #endregion

        #region Private

        private BSScope()
        {

            m_LocalVars.InsertElement( new BSObject( "__SELF" ), new BSObject( this ) );
            m_LocalVars.InsertElement( new BSObject( "__L" ), m_LocalVars );
        }

        #endregion
    }

}

using System.Collections.Generic;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Types.References.Implementations
{

    public class BSTableReference : ABSReference
    {
        private readonly ABSTable m_SourceTable;
        private readonly ABSObject m_Key;
        

        #region Public

        public BSTableReference( ABSTable table, ABSObject key )
        {
            m_SourceTable = table;
            m_Key = key;
        }

        public override void Assign( ABSObject obj )
        {
            m_SourceTable.InsertElement( m_Key, obj );
        }

       
        public override ABSObject Get()
        {
            ABSObject k = m_Key.ResolveReference();

            if ( m_SourceTable.HasElement( k ) )
            {
                return m_SourceTable.GetRawElement( k );
            }

            return new BSObject( null );
        }

        
        #endregion
    }

}

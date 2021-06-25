using System.Collections.Generic;
using BadScript.Common.Exceptions;

namespace BadScript.Common.Types.References.Implementations
{

    public class BSArrayReference : ABSReference
    {
        private readonly ABSArray m_SourceTable;
        private readonly int m_Key;

        public override bool IsNull => Get().IsNull;

        #region Public

        public BSArrayReference( ABSArray table, int key )
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
            if ( m_SourceTable.GetLength() > m_Key )
            {
                return m_SourceTable.GetRawElement( m_Key );
            }

            throw new BSRuntimeException( $"Index is out of bounds: {m_Key}" );
        }
        #endregion
    }

}

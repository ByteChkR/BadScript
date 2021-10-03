using BadScript.Exceptions;

namespace BadScript.Types.References.Implementations
{

    public class BSArrayReference : ABSReference
    {

        private readonly ABSArray m_SourceTable;
        private readonly int m_Key;
        private readonly bool m_ReadOnly;

        public override bool IsNull => Get().IsNull;

        #region Public

        public BSArrayReference( ABSArray table, int key, bool readOnly ) : base( table.Position )
        {
            m_SourceTable = table;
            m_Key = key;
            m_ReadOnly = readOnly;
        }

        public override void Assign( ABSObject obj )
        {
            if ( m_ReadOnly )
            {
                throw new BSRuntimeException(
                                             Position,
                                             $"Array '{m_SourceTable.SafeToString()}' is locked and can not be edited"
                                            );
            }

            m_SourceTable.InsertElement( m_Key, obj );
        }

        public override ABSObject Get()
        {
            if ( m_SourceTable.GetLength() > m_Key )
            {
                return m_SourceTable.GetRawElement( m_Key );
            }

            throw new BSRuntimeException( Position, $"Index is out of bounds: {m_Key}" );
        }

        #endregion

    }

}

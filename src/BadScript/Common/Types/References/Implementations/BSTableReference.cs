using BadScript.Common.Exceptions;

namespace BadScript.Common.Types.References.Implementations
{

    public class BSTableReference : ABSReference
    {

        private readonly ABSTable m_SourceTable;
        private readonly ABSObject m_Key;
        private readonly bool m_ReadOnly;

        #region Public

        public BSTableReference( ABSTable table, ABSObject key, bool readOnly ) : base( table.Position )
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
                                             $"Table '{m_SourceTable.SafeToString()}.{m_Key.SafeToString()}' is locked and can not be edited"
                                            );
            }

            m_SourceTable.InsertElement( m_Key, obj );
        }

        public override ABSObject Get()
        {
            ABSObject k = m_Key.ResolveReference();

            if ( m_SourceTable.HasElement( k ) )
            {
                return m_SourceTable.GetRawElement( k );
            }

            throw new BSRuntimeException(
                                         m_Key.Position,
                                         $"Property '{k.SafeToString()}' is null in table '{m_SourceTable.SafeToString()}'"
                                        );
        }

        #endregion

    }

}

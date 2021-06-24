using System.Collections.Generic;

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

        public override bool Equals( ABSObject other )
        {
            return Get().Equals( other );
        }

        public override ABSObject Get()
        {
            ABSObject k = m_Key.ResolveReference();

            return m_SourceTable.GetRawElement( k );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return Get().GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return Get().HasProperty( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            return Get().Invoke( args );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return Get().SafeToString( doneList );
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            Get().SetProperty( propertyName, obj );
        }

        public override bool TryConvertBool( out bool v )
        {
            return Get().TryConvertBool( out v );
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            return Get().TryConvertDecimal( out d );
        }

        public override bool TryConvertString( out string v )
        {
            return Get().TryConvertString( out v );
        }

        #endregion
    }

}

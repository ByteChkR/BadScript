using System.Collections.Generic;

namespace BadScript.Runtime.Implementations
{

    public class BSRuntimeTableReference : BSRuntimeReference
    {

        private readonly BSRuntimeTable m_SourceTable;
        private readonly BSRuntimeObject m_Key;

        #region Public

        public BSRuntimeTableReference( BSRuntimeTable table, BSRuntimeObject key )
        {
            m_SourceTable = table;
            m_Key = key;
        }

        public override void Assign( BSRuntimeObject obj )
        {
            m_SourceTable.InsertElement( m_Key, obj );
        }

        public override bool Equals( BSRuntimeObject other )
        {
            return Get().Equals( other );
        }

        public override BSRuntimeObject Get()
        {
            BSRuntimeObject k = m_Key;

            while ( k is BSRuntimeReference r )
            {
                k = r.Get();
            }

            return m_SourceTable.GetRawElement( k );
        }

        public override BSRuntimeReference GetProperty( string propertyName )
        {
            return Get().GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return Get().HasProperty( propertyName );
        }

        public override BSRuntimeObject Invoke( BSRuntimeObject[] args )
        {
            return Get().Invoke( args );
        }

        public override string SafeToString( Dictionary < BSRuntimeObject, string > doneList )
        {
            return Get().SafeToString( doneList );
        }

        public override void SetProperty( string propertyName, BSRuntimeObject obj )
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

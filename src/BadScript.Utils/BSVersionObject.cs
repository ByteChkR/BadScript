using System;
using System.Collections.Generic;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Utils.Reflection;

namespace BadScript.Utils
{

    public class BSVersionObject : BSObject
    {
        private Dictionary < string, ABSReference > m_Properties;

        #region Public

        public BSVersionObject( Version instance ) : base( instance )
        {
            LoadProperties();
        }

        public override ABSReference GetProperty( string propertyName )
        {
            if ( m_Properties.ContainsKey( propertyName ) )
            {
                return m_Properties[propertyName];
            }

            return base.GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_Properties.ContainsKey( propertyName ) || base.HasProperty( propertyName );
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        #endregion

        #region Private

        private ABSObject ChangeVersion( ABSObject[] arg )
        {
            Version v = ( Version ) m_InternalObject;

            return new BSVersionObject( v.ChangeVersion( arg[0].ConvertString() ) );
        }

        private ABSObject GetBuild()
        {
            return new BSObject( ( ( Version ) m_InternalObject ).Build );
        }

        private ABSObject GetMajor()
        {
            return new BSObject( ( ( Version ) m_InternalObject ).Major );
        }

        private ABSObject GetMajorRevision()
        {
            return new BSObject( ( ( Version ) m_InternalObject ).MajorRevision );
        }

        private ABSObject GetMinor()
        {
            return new BSObject( ( ( Version ) m_InternalObject ).Minor );
        }

        private ABSObject GetMinorRevision()
        {
            return new BSObject( ( ( Version ) m_InternalObject ).MinorRevision );
        }

        private ABSObject GetRevision()
        {
            return new BSObject( ( ( Version ) m_InternalObject ).Revision );
        }

        private void LoadProperties()
        {
            m_Properties = new Dictionary < string, ABSReference >();
            m_Properties.Add( "major", new BSReflectionReference( GetMajor, null ) );
            m_Properties.Add( "minor", new BSReflectionReference( GetMinor, null ) );
            m_Properties.Add( "majorRevision", new BSReflectionReference( GetMajorRevision, null ) );
            m_Properties.Add( "minorRevision", new BSReflectionReference( GetMinorRevision, null ) );
            m_Properties.Add( "revision", new BSReflectionReference( GetRevision, null ) );
            m_Properties.Add( "build", new BSReflectionReference( GetBuild, null ) );

            m_Properties.Add(
                "change",
                new BSFunctionReference( new BSFunction( "function change(changeStr)", ChangeVersion, 1 ) ) );

        }

        #endregion
    }

}

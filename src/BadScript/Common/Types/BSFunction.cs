using System;
using System.Collections.Generic;

using BadScript.Common.Types.References;

namespace BadScript.Common.Types
{

    public class BSFunction : ABSObject
    {

        private readonly string m_DebugData = null;

        private Func < ABSObject[],
            ABSObject > m_Func;

        #region Public

        public BSFunction(
            string debugData,
            Func < ABSObject[], ABSObject >
                func )
        {
            m_DebugData = debugData;
            m_Func = func;
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new Exception( $"Property {propertyName} does not exist" );
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            return m_Func( args );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return m_DebugData ?? m_Func.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new Exception( $"Property {propertyName} does not exist" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = true;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 1;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = m_DebugData ?? m_Func.ToString();
            ;

            return true;
        }

        #endregion

    }

}

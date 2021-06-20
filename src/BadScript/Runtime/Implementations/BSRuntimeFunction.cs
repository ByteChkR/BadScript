using System;
using System.Collections.Generic;

namespace BadScript.Runtime.Implementations
{

    public class BSRuntimeFunction : BSRuntimeObject
    {
        
        private readonly string m_DebugData = null;

        private Func < BSRuntimeObject[],
            BSRuntimeObject > m_Func;

        #region Public

        public BSRuntimeFunction(
            string debugData,
            Func < BSRuntimeObject[], BSRuntimeObject >
                func )
        {
            m_DebugData = debugData;
            m_Func = func;
        }


        public override bool Equals( BSRuntimeObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override BSRuntimeReference GetProperty( string propertyName )
        {
            throw new Exception( $"Property {propertyName} does not exist" );
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override BSRuntimeObject Invoke( BSRuntimeObject[] args )
        {
            return m_Func( args );
        }

        public override string SafeToString( Dictionary < BSRuntimeObject, string > doneList )
        {
            return m_DebugData ?? m_Func.ToString();
        }

        public override void SetProperty( string propertyName, BSRuntimeObject obj )
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

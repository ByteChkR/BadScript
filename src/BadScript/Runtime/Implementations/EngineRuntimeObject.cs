using System;
using System.Collections.Generic;

namespace BadScript.Runtime.Implementations
{

    public class EngineRuntimeObject : BSRuntimeObject
    {

        private readonly object m_InternalObject;

        #region Public

        public EngineRuntimeObject( object instance )
        {
            m_InternalObject = instance;
        }

        public override bool Equals( BSRuntimeObject other )
        {
            if ( m_InternalObject == null )
            {
                return other is EngineRuntimeObject oN && oN.m_InternalObject == null;
            }

            return other is EngineRuntimeObject o && m_InternalObject.Equals( o.m_InternalObject );
        }

        public object GetInternalObject()
        {
            return m_InternalObject;
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
            throw new Exception( $"Can not invoke '{this}'" );
        }

        public override string SafeToString( Dictionary < BSRuntimeObject, string > doneList )
        {
            if ( doneList.ContainsKey( this ) )
            {
                return doneList[this];
            }
            else
            {
                return doneList[this] = m_InternalObject?.ToString() ?? "NULL";
            }
        }

        public override void SetProperty( string propertyName, BSRuntimeObject obj )
        {
            throw new Exception( $"Property {propertyName} does not exist" );
        }

        public override string ToString()
        {
            return m_InternalObject?.ToString() ?? "NULL";
        }

        public override bool TryConvertBool( out bool v )
        {
            v = false;

            if ( m_InternalObject is decimal d )
            {
                v = d != 0;
            }
            else if ( m_InternalObject != null )
            {
                v = true;
            }

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            if ( m_InternalObject is decimal dV )
            {
                d = dV;
            }
            else if ( m_InternalObject != null )
            {
                d = 1;
            }

            return !(m_InternalObject is string);
        }

        public override bool TryConvertString( out string v )
        {
            v = m_InternalObject?.ToString() ?? "NULL";

            return true;
        }

        #endregion

    }

}

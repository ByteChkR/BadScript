using System.Collections.Generic;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types.References;

namespace BadScript.Common.Types.Implementations
{

    public class BSObject : ABSObject
    {
        public static readonly BSObject Null = new BSObject( null );
        public static readonly BSObject One = new BSObject( ( decimal ) 1 );
        public static readonly BSObject Zero = new BSObject( ( decimal ) 0 );
        
        protected readonly object m_InternalObject;
        public override bool IsNull => m_InternalObject == null;

        #region Public

        public BSObject( object instance ) : this( SourcePosition.Unknown, instance )
        {
        }

        public BSObject( SourcePosition pos, object instance ) : base( pos )
        {
            m_InternalObject = instance;
        }

        public override bool Equals( ABSObject other )
        {
            if ( m_InternalObject == null )
            {
                return other is BSObject oN && oN.m_InternalObject == null;
            }

            return other is BSObject o && m_InternalObject.Equals( o.m_InternalObject );
        }

        public object GetInternalObject()
        {
            return m_InternalObject;
        }

        public override ABSReference GetProperty( string propertyName )
        {

            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( Position, $"Can not invoke '{this}'" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
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

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist" );
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

            return !( m_InternalObject is string );
        }

        public override bool TryConvertString( out string v )
        {
            v = m_InternalObject?.ToString() ?? "NULL";

            return true;
        }

        #endregion
    }

}

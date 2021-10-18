using System.Collections.Generic;
using System.Runtime.CompilerServices;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types.References;

namespace BadScript.Types.Implementations
{

    public class BSObject : ABSObject, IBSWrappedObject
    {

        public static readonly BSObject Null = new BSObject(null);
        public static readonly BSObject EmptyString = new BSObject(string.Empty);
        public static readonly BSObject True = new BSObject( true );
        public static readonly BSObject False = new BSObject( false );

        protected readonly object m_InternalObject;

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
            if ( other == null )
            {
                return false;
            }

            if ( IsNull() )
            {
                return other.IsNull();
            }

            if ( other is BSObject oN )
            {
                return m_InternalObject.Equals( oN.m_InternalObject );
            }

            return false;
        }

        public object GetInternalObject()
        {
            return m_InternalObject;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist in object {SafeToString()}" );
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( Position, $"Can not invoke '{this}'" );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsLiteral()
        {
            return IsNull() ||
                   m_InternalObject is decimal or string or bool;
        }

        public override bool IsNull()
        {
            return m_InternalObject == null;
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
            throw new BSRuntimeException( Position, $"Property {propertyName} does not exist in object {SafeToString()}" );
        }

        public override string ToString()
        {
            return m_InternalObject?.ToString() ?? "NULL";
        }

        public override bool TryConvertBool( out bool v )
        {
            if ( m_InternalObject is bool b )
            {
                v = b;

                return true;
            }

            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            if ( m_InternalObject is decimal dV )
            {
                d = dV;

                return true;
            }

            d = decimal.Zero;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            if ( m_InternalObject is string s )
            {
                v = s;

                return true;
            }

            v = null;

            return false;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_InternalObject?.GetHashCode() ?? 0;
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;

using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Common.Types
{

    public abstract class ABSObject : IEquatable < ABSObject >
    {

        public static bool operator ==( ABSObject left, ABSObject right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( ABSObject left, ABSObject right )
        {
            return !Equals( left, right );
        }

        #region Public

        public abstract bool Equals( ABSObject other );

        public abstract ABSReference GetProperty( string propertyName );

        public abstract bool HasProperty( string propertyName );

        public abstract ABSObject Invoke( ABSObject[] args );

        public abstract string SafeToString( Dictionary < ABSObject, string > doneList );

        public abstract void SetProperty( string propertyName, ABSObject obj );

        public abstract bool TryConvertBool( out bool v );

        public abstract bool TryConvertDecimal( out decimal d );

        public abstract bool TryConvertString( out string v );

        public bool ConvertBool()
        {
            if ( TryConvertBool( out bool r ) )
            {
                return r;
            }

            throw new Exception( $"Can not Convert object '{this}' to type '{typeof( bool )}'" );
        }

        public decimal ConvertDecimal()
        {
            if ( TryConvertDecimal( out decimal r ) )
            {
                return r;
            }

            throw new Exception( $"Can not Convert object '{this}' to type '{typeof( decimal )}'" );
        }

        public string ConvertString()
        {
            if ( TryConvertString( out string r ) )
            {
                return r;
            }

            throw new Exception( $"Can not Convert object '{this}' to type '{typeof( string )}'" );
        }

        public override bool Equals( object obj )
        {
            return obj is ABSObject o && Equals( o );
        }

        public ABSReference GetOrAddProperty( string propertyName )
        {
            if ( !HasProperty( propertyName ) )
            {
                SetProperty( propertyName, new BSObject( null ) );
            }

            return GetProperty( propertyName );
        }

        public string SafeToString()
        {
            return SafeToString( new Dictionary < ABSObject, string >() );
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;

using BadScript.Runtime.Implementations;

namespace BadScript.Runtime
{

    public abstract class BSRuntimeObject : IEquatable < BSRuntimeObject >
    {

        public static bool operator ==( BSRuntimeObject left, BSRuntimeObject right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( BSRuntimeObject left, BSRuntimeObject right )
        {
            return !Equals( left, right );
        }

        #region Public

        public abstract bool Equals( BSRuntimeObject other );

        public abstract BSRuntimeReference GetProperty( string propertyName );

        public abstract bool HasProperty( string propertyName );

        public abstract BSRuntimeObject Invoke( BSRuntimeObject[] args );

        public abstract string SafeToString( Dictionary < BSRuntimeObject, string > doneList );

        public abstract void SetProperty( string propertyName, BSRuntimeObject obj );

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

        public string ConvertString()
        {
            if (TryConvertString(out string r))
            {
                return r;
            }

            throw new Exception($"Can not Convert object '{this}' to type '{typeof(string)}'");
        }
        public decimal ConvertDecimal()
        {
            if (TryConvertDecimal(out decimal r))
            {
                return r;
            }

            throw new Exception($"Can not Convert object '{this}' to type '{typeof(decimal)}'");
        }

        public override bool Equals( object obj )
        {
            return obj is BSRuntimeObject o && Equals( o );
        }

        public BSRuntimeReference GetOrAddProperty( string propertyName )
        {
            if ( !HasProperty( propertyName ) )
            {
                SetProperty( propertyName, new EngineRuntimeObject( null ) );
            }

            return GetProperty( propertyName );
        }

        public string SafeToString()
        {
            return SafeToString( new Dictionary < BSRuntimeObject, string >() );
        }

        #endregion

    }

}

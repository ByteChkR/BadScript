using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types.References;

namespace BadScript.Common.Types
{

    public abstract class ABSObject : IEquatable < ABSObject >
    {
        public readonly SourcePosition Position;

        public static bool operator ==( ABSObject left, ABSObject right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( ABSObject left, ABSObject right )
        {
            return !Equals( left, right );
        }

        public abstract bool IsNull { get; }

        #region Public

        public abstract bool Equals( ABSObject other );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract ABSReference GetProperty( string propertyName );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool HasProperty( string propertyName );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract ABSObject Invoke( ABSObject[] args );

        public abstract string SafeToString( Dictionary < ABSObject, string > doneList );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SetProperty( string propertyName, ABSObject obj );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool TryConvertBool( out bool v );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool TryConvertDecimal( out decimal d );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool TryConvertString( out string v );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConvertBool()
        {
            if ( TryConvertBool( out bool r ) )
            {
                return r;
            }

            throw new BSInvalidTypeException( Position, $"Can not Convert object.", this, "boolean" );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal ConvertDecimal()
        {
            if ( TryConvertDecimal( out decimal r ) )
            {
                return r;
            }

            throw new BSInvalidTypeException( Position, $"Can not Convert object.", this, "number" );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ConvertString()
        {
            if ( TryConvertString( out string r ) )
            {
                return r;
            }

            throw new BSInvalidTypeException( Position, $"Can not Convert object.", this, "string" );
        }

        public override bool Equals( object obj )
        {
            return obj is ABSObject o && Equals( o );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SafeToString()
        {
            return SafeToString( new Dictionary < ABSObject, string >() );
        }

        #endregion

        #region Protected

        protected ABSObject( SourcePosition pos )
        {
            Position = pos;
        }

        #endregion
    }

}

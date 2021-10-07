using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types.References;

namespace BadScript.Types
{

    /// <summary>
    ///     Base Implementation for all objects inside the language
    /// </summary>
    public abstract class ABSObject : IEquatable < ABSObject >
    {

        /// <summary>
        ///     Source Position(can be unknown)
        /// </summary>
        public readonly SourcePosition Position;

        public static bool operator ==( ABSObject left, ABSObject right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( ABSObject left, ABSObject right )
        {
            return !Equals( left, right );
        }

        public override int GetHashCode()
        {
            return GetHashCodeImpl();
        }

        protected abstract int GetHashCodeImpl();

        /// <summary>
        ///     Returns true if the object is null
        /// </summary>
        public abstract bool IsNull();

        #region Public

        /// <summary>
        ///     Returns true if the other ABSObject is equal to this object
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool Equals( ABSObject other );

        /// <summary>
        ///     Returns a reference to the specified property
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract ABSReference GetProperty( string propertyName );

        /// <summary>
        ///     Returns true if a the specified property exists.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract bool HasProperty( string propertyName );

        /// <summary>
        ///     Invokes the current object with the specified arguments
        /// </summary>
        /// <param name="args">Arguments for the invocation</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract ABSObject Invoke( ABSObject[] args );

        /// <summary>
        ///     Returns a verbose string representation of this object
        /// </summary>
        /// <param name="doneList">List of Done Objects to prevent recursion</param>
        /// <returns></returns>
        public abstract string SafeToString( Dictionary < ABSObject, string > doneList );

        /// <summary>
        ///     Sets a Property to a specified value
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <param name="obj">Value</param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract void SetProperty( string propertyName, ABSObject obj );

        /// <summary>
        ///     Tries to convert the object into a boolean value
        /// </summary>
        /// <param name="v">Converted Value</param>
        /// <returns>True if operation succeeded</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract bool TryConvertBool( out bool v );

        /// <summary>
        ///     Tries to convert the object into a decimal value
        /// </summary>
        /// <param name="d">Converted Value</param>
        /// <returns>True if operation succeeded</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract bool TryConvertDecimal( out decimal d );

        /// <summary>
        ///     Tries to convert the object into a string value
        /// </summary>
        /// <param name="s">Converted Value</param>
        /// <returns>True if operation succeeded</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract bool TryConvertString( out string v );

        /// <summary>
        ///     Converts the Object into a boolean
        /// </summary>
        /// <returns>The Converted Object</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool ConvertBool()
        {
            if ( TryConvertBool( out bool r ) )
            {
                return r;
            }

            throw new BSInvalidTypeException( Position, $"Can not Convert object.", this, "boolean" );
        }

        /// <summary>
        ///     Converts the Object into a decimal
        /// </summary>
        /// <returns>The Converted Object</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public decimal ConvertDecimal()
        {
            if ( TryConvertDecimal( out decimal r ) )
            {
                return r;
            }

            throw new BSInvalidTypeException( Position, $"Can not Convert object.", this, "number" );
        }

        /// <summary>
        ///     Converts the Object into a string
        /// </summary>
        /// <returns>The Converted Object</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
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

        /// <summary>
        ///     Returns a verbose string representation of this object
        /// </summary>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
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

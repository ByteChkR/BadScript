using System;
using System.Collections;
using System.Collections.Generic;

using BadScript.Parser.Expressions;
using BadScript.Types.References;

namespace BadScript.Types
{

    /// <summary>
    ///     Base Implementations for all arrays inside the language
    /// </summary>
    public abstract class ABSArray : ABSObject, IEnumerable < ABSObject >
    {

        #region Public

        /// <summary>
        ///     Runs the specified func for each element inside the array
        /// </summary>
        /// <typeparam name="T">Return type of Func</typeparam>
        /// <param name="o">Func to execute</param>
        /// <returns>Enumeration of Results of the invocations of func</returns>
        public abstract IEnumerable < T > ForEach < T >( Func < ABSObject, T > o );

        /// <summary>
        ///     Returns a reference to an element at the specified index
        /// </summary>
        /// <param name="i">Index of the Element</param>
        /// <returns></returns>
        public abstract ABSReference GetElement( int i );

        /// <summary>
        ///     Returns the size of all elements inside this array
        /// </summary>
        /// <returns></returns>
        public abstract int GetLength();

        /// <summary>
        ///     Returns the Value of an element at the specified index
        /// </summary>
        /// <param name="i">Index of the Element</param>
        public abstract ABSObject GetRawElement( int i );

        /// <summary>
        ///     Inserts an element at the specified index
        /// </summary>
        /// <param name="i">Index of the Element</param>
        /// <param name="o">Value</param>
        public abstract void InsertElement( int i, ABSObject o );

        /// <summary>
        ///     Removes an element at the specified index
        /// </summary>
        /// <param name="k">Index of the Element</param>
        public abstract void RemoveElement( int i );

        /// <summary>
        ///     Sets an element at the specified index
        /// </summary>
        /// <param name="k">Index of the Element</param>
        /// <param name="o">Value</param>
        public abstract void SetElement( int k, ABSObject o );

        /// <summary>
        ///     Adds an element to the array
        /// </summary>
        /// <param name="o">Element to add</param>
        public void AddElement( ABSObject o )
        {
            InsertElement( GetLength(), o );
        }

        /// <summary>
        ///     Clears the Content of the Array
        /// </summary>
        public virtual void Clear()
        {
            while ( GetLength() != 0 )
            {
                RemoveElement( 0 );
            }
        }

        public IEnumerator < ABSObject > GetEnumerator()
        {
            for ( int i = 0; i < GetLength(); i++ )
            {
                yield return GetRawElement( i );
            }
        }

        #endregion

        #region Protected

        protected ABSArray( SourcePosition pos ) : base( pos )
        {
        }

        #endregion

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}

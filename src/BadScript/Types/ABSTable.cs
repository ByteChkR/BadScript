using BadScript.Parser.Expressions;
using BadScript.Types.References;

namespace BadScript.Types
{

    /// <summary>
    ///     Base Implementation for all tables inside the language
    /// </summary>
    public abstract class ABSTable : ABSObject
    {

        /// <summary>
        ///     Contains an ABSArray of all keys in this table
        /// </summary>
        public abstract ABSArray Keys { get; }

        /// <summary>
        ///     Contains an ABSArray of all values in this table
        /// </summary>
        public abstract ABSArray Values { get; }

        #region Public

        /// <summary>
        ///     Clears all Elements in the Table
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Gets an element reference with the specified key
        /// </summary>
        /// <param name="i">The Key</param>
        /// <returns></returns>
        public abstract ABSReference GetElement( ABSObject i );

        /// <summary>
        ///     Gets the count of all elements inside the table
        /// </summary>
        /// <returns></returns>
        public abstract int GetLength();

        /// <summary>
        ///     Gets an element with the specified key
        /// </summary>
        /// <param name="k">The Key</param>
        /// <returns></returns>
        public abstract ABSObject GetRawElement(ABSObject k);
        /// <summary>
        ///     Gets an element with the specified key
        /// </summary>
        /// <param name="k">The Key</param>
        /// <returns></returns>
        public abstract void SetRawElement(ABSObject k, ABSObject o);

        /// <summary>
        ///     Returns true if an element with the specified key exists
        /// </summary>
        /// <param name="i">The Key</param>
        /// <returns></returns>
        public abstract bool HasElement( ABSObject i );

        /// <summary>
        ///     Inserts a key value pair
        /// </summary>
        /// <param name="k">Key</param>
        /// <param name="o">Value</param>
        public abstract void InsertElement( ABSObject k, ABSObject o );

        /// <summary>
        ///     Removes an element with the specified key
        /// </summary>
        /// <param name="k"></param>
        public abstract void RemoveElement( ABSObject k );

        #endregion

        #region Protected

        protected ABSTable( SourcePosition pos ) : base( pos )
        {
        }

        #endregion

    }

}

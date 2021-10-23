using BadScript.Scopes;
using BadScript.Types;

namespace BadScript.Parser.Expressions
{

    /// <summary>
    ///     Abstract base of all Expressions that are used.
    /// </summary>
    public abstract class BSExpression
    {

        /// <summary>
        ///     The Source Position
        ///     Unknown for Deserialized and Custom Expressions
        /// </summary>
        protected readonly SourcePosition m_Position;

        public SourcePosition Position => m_Position;
        /// <summary>
        ///     True if an expression can be computed once because the result never changes
        /// </summary>
        public abstract bool IsConstant { get; }

        #region Public

        /// <summary>
        ///     Gets called to invoke this (and all sub expressions)
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public abstract ABSObject Execute( BSScope scope );

        #endregion

        #region Protected

        protected BSExpression( SourcePosition pos )
        {
            m_Position = pos;
        }

        #endregion

    }

}

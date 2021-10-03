using BadScript.Types;

namespace BadScript.Interfaces
{

    /// <summary>
    ///     Abstract base of all Script Interfaces that are used.
    /// </summary>
    public abstract class ABSScriptInterface
    {

        /// <summary>
        ///     The Interface Name / Interface key
        /// </summary>
        public string Name { get; }

        #region Public

        /// <summary>
        ///     The Function that gets called when the Script Interface is beeing loaded.
        /// </summary>
        /// <param name="root">The Root Table that the interface will add its items to.</param>
        public abstract void AddApi( ABSTable root );

        #endregion

        #region Protected

        protected ABSScriptInterface( string name )
        {
            Name = name;
        }

        #endregion

    }

}

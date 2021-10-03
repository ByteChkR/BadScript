using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Interfaces
{

    public static class ABSTableInterfaceExtensions
    {

        public static void InsertElement(this ABSTable t, string key, ABSObject value) =>
            t.InsertElement(new BSObject(key), value);
        public static void InsertElement(this ABSTable t, string key, string value) =>
            t.InsertElement(new BSObject(key), new BSObject(value));

    }
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

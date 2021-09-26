using BadScript.Common.Types;

namespace BadScript.Interfaces
{

    public abstract class ABSScriptInterface
    {

        public string Name { get; }

        #region Public

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

using BadScript.Common.Types;

namespace BadScript
{

    public abstract class ABSScriptInterface
    {
        public string Name { get; }
        protected ABSScriptInterface(string name) => Name = name;

        public abstract void AddApi(ABSTable root);
    }

}
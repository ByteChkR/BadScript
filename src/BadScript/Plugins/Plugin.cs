using System;

namespace BadScript.Plugins
{
    public abstract class Plugin<T> : Plugin
    {
        protected Plugin(string name, string description, string author, Version version) : base(name, description, author, version)
        {
        }


        public abstract void Load(T item);
        public override bool Load(object item)
        {
            if (item is T i)
            {
                Load(i);
                return true;
            }
            return false;
        }
    }
}
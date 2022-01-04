using System;

namespace BadScript.Plugins
{
    public abstract class Plugin
    {
        protected Plugin(string name, string description, string author, Version version)
        {
            Name = name;
            Description = description;
            Author = author;
            Version = version;
        }

        public string Name { get; }
        public string Description { get; }
        public string Author { get; }
        public Version Version { get; }

        public abstract bool Load(object item);

        public override string ToString()
        {
            return $"{Name}@{Version}";
        }
    }
}
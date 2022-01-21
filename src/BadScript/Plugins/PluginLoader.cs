using System;

namespace BadScript.Plugins
{

    public abstract class Plugin
    {

        public string Name { get; }

        public string Description { get; }

        public string Author { get; }

        public Version Version { get; }

        #region Public

        public abstract bool Load( object item );

        public override string ToString()
        {
            return $"{Name}@{Version}";
        }

        #endregion

        #region Protected

        protected Plugin( string name, string description, string author, Version version )
        {
            Name = name;
            Description = description;
            Author = author;
            Version = version;
        }

        #endregion

    }

}

using System;

namespace BadScript.Plugins
{

    public abstract class Plugin < T > : Plugin
    {

        #region Public

        public abstract void Load( T item );

        public override bool Load( object item )
        {
            if ( item is T i )
            {
                Load( i );

                return true;
            }

            return false;
        }

        #endregion

        #region Protected

        protected Plugin( string name, string description, string author, Version version ) : base(
             name,
             description,
             author,
             version
            )
        {
        }

        #endregion

    }

}

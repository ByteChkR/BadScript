using System;

using BadScript.Common.Types;

namespace BadScript.Utils.Reflection
{

    public static class TypeBuilder < T >
    {

        #region Public

        public static void AddFilterType()
        {
            TypeBuilder.AddFilterType( typeof( T ) );
        }

        public static ABSObject Build( object instance = null )
        {
            return TypeBuilder.Build( typeof( T ), instance );
        }

        #endregion

    }

    public static class TypeBuilder
    {

        #region Public

        public static void AddFilterType( Type t )
        {
            TypeBuilderData.AddFilterType( t );
        }

        public static ABSObject Build( Type t, object instance = null )
        {
            TypeBuilderData tdata = TypeBuilderData.GetData( t );

            return tdata.WrapObject( instance );
        }

        public static void Expand()
        {
            TypeBuilderData.Expand();
        }

        public static void ExpandAll()
        {
            TypeBuilderData.ExpandAll();
        }

        public static ABSTable GetConstructorData()
        {
            return TypeBuilderData.GetConstructorData();
        }

        public static void SetFilterType( TypeBuilderTypeFilter filter )
        {
            TypeBuilderData.SetFilterType( filter );
        }

        #endregion

    }

}

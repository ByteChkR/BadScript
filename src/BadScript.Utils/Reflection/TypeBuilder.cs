using System;
using BadScript.Common.Types;

namespace BadScript.Utils
{

    public static class TypeBuilder<T>
    {
        public static ABSObject Build(object instance = null)
        {
            return TypeBuilder.Build(typeof(T), instance);
        }
        
        public static void AddFilterType() => TypeBuilder.AddFilterType( typeof( T ) );

    }
    public static class TypeBuilder
    {
        public static ABSTable GetConstructorData() => TypeBuilderData.GetConstructorData();
        public static void Expand() => TypeBuilderData.Expand();

        public static void ExpandAll() => TypeBuilderData.ExpandAll();

        public static void AddFilterType( Type t ) => TypeBuilderData.AddFilterType( t );

        public static void SetFilterType( TypeBuilderTypeFilter filter ) => TypeBuilderData.SetFilterType( filter );
        public static ABSObject Build(Type t, object instance = null)
        {
            TypeBuilderData tdata = TypeBuilderData.GetData( t );

            return tdata.WrapObject( instance );
        }

    }

}

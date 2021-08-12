﻿using BadScript.Common.Types;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public static class WrapperConstructorDataBaseExtensions
    {
        public static ABSObject Get<T>(this IWrapperConstructorDataBase db, object[] args )
        {
            return db.Get( typeof( T ), args );
        }
    }

}
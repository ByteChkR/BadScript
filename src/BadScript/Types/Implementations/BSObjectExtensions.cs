﻿using System;
using System.Collections.Generic;

namespace BadScript.Types.Implementations
{

    internal static class BSObjectExtensions
    {

        private static readonly Dictionary < Type, BSObjectExtension > s_Extensions = new();
        public static void AddExtension( Type t, BSObjectExtension ext ) => s_Extensions.Add( t, ext );
        public static void AddExtension<T>( BSObjectExtension ext ) => AddExtension( typeof(T), ext );

        public static ABSObject GetProperty( BSObject o, string name ) => s_Extensions[o.GetInternalObject().GetType()].GetProperty( name, o );
        public static bool HasProperty( Type t, string name ) => s_Extensions[t].HasProperty( name );
        
    }

}
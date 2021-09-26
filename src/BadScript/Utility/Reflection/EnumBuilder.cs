﻿using System;

using BadScript.Common.Expressions;
using BadScript.Common.Types.Implementations;

namespace BadScript.Utility.Reflection
{

    public static class EnumBuilder < T > where T : Enum
    {

        #region Public

        public static BSTable Build()
        {
            return EnumBuilder.Build( typeof( T ) );
        }

        #endregion

    }

    public static class EnumBuilder
    {

        #region Public

        public static BSTable Build( Type type )
        {
            BSTable t = new BSTable( SourcePosition.Unknown );
            string[] keys = Enum.GetNames( type );

            foreach ( string key in keys )
            {
                int v = ( int )Enum.Parse( type, key );
                t.InsertElement( new BSObject( key ), new BSObject( ( decimal )v ) );
            }

            return t;
        }

        #endregion

    }

}
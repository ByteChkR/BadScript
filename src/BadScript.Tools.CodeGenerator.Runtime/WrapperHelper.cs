using System;
using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Interfaces;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public static class WrapperHelper
    {
        public static bool AllowRecurseToString = true;

        private static readonly List < IWrapperConstructorDataBase > s_DataBases =
            new List < IWrapperConstructorDataBase >();

        #region Public
        


        public static ABSScriptInterface CreateInterface(this WrapperStaticDataBase db, string name)
        {
            return new BSConstantScriptInterface(
                name,
                table =>
                {
                    foreach (KeyValuePair < Type, BSStaticWrapperObject> dbType in db.StaticTypes)
                    {
                        ABSTable t = table.HasElement(new BSObject(dbType.Key.Name))
                            ? (ABSTable)table.GetRawElement(new BSObject(dbType.Key.Name)).ResolveReference()
                            : new BSTable(SourcePosition.Unknown);

                        table.InsertElement(new BSObject(dbType.Key.Name), t);

                        foreach ( string valueProperty in dbType.Value.Properties )
                        {
                            t.InsertElement(
                                new BSObject(valueProperty),
                                dbType.Value.GetProperty(valueProperty));
                        }
                    }

                });
        }

        public static ABSScriptInterface CreateInterface( this IWrapperConstructorDataBase db, string name )
        {
            return new BSConstantScriptInterface(
                name,
                table =>
                {
                    foreach ( Type dbType in db.Types )
                    {
                        ABSTable t = table.HasElement( new BSObject( dbType.Name ) )
                            ? (ABSTable)table.GetRawElement( new BSObject( dbType.Name ) ).ResolveReference()
                            : new BSTable( SourcePosition.Unknown );

                        table.InsertElement( new BSObject( dbType.Name ), t );
                        t.InsertElement(
                            new BSObject( dbType.Name ),
                            new
                                BSFunction(
                                    $"function {dbType.Name}(args)",
                                    objects => db.Get(
                                        dbType,
                                        objects.Select( UnwrapObject < object > ).
                                                ToArray() ),
                                    0,
                                    int.MaxValue ) );
                    }

                } );
        }

        public static object UnwrapObject( Type t, ABSObject o )
        {
            if ( o is IBSWrappedObject obj )
            {
                object oi = obj.GetInternalObject();

                if ( t == typeof( bool ) )
                {
                    return o.ConvertBool();
                }
                else if ( t == typeof( decimal ) )
                {
                    return o.ConvertDecimal();
                }
                else if ( t == typeof( string ) )
                {
                    return o.ConvertString();
                }

                return Convert.ChangeType( oi, t );
            }

            throw new BSRuntimeException( "Can not Unwrap Object" );
        }

        public static T UnwrapObject < T >( ABSObject o )
        {
            return ( T ) UnwrapObject( typeof( T ), o );
        }

        #endregion
    }

}

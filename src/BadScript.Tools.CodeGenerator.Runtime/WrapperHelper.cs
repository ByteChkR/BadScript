using System;
using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Interfaces;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public static class WrapperHelper
    {
        private static readonly List < IWrapperConstructorDataBase > s_DataBases =
            new List < IWrapperConstructorDataBase >();

        #region Public

        public static void AddObjectDB( IWrapperConstructorDataBase db )
        {
            if ( s_DataBases.Contains( db ) )
            {
                return;
            }

            s_DataBases.Add( db );
        }

        public static ABSObject Create < T >( object[] args )
        {
            IWrapperConstructorDataBase db = s_DataBases.FirstOrDefault( x => x.HasType < T >() );

            if ( db != null )
            {
                return db.Get < T >( args );
            }

            throw new Exception( "Type not Found" );
        }

        public static ABSScriptInterface CreateInterface( this IWrapperConstructorDataBase db, string name )
        {
            return new BSConstantScriptInterface(
                name,
                table =>
                {
                    foreach ( Type dbType in db.Types )
                    {
                        table.InsertElement(
                            new BSObject( dbType.Name ),
                            new
                                BSFunction(
                                    $"function {dbType.Name}(args)",
                                    objects => db.Get(
                                        dbType,
                                        objects.Select( x => UnwrapObject < object >( x ) ).
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

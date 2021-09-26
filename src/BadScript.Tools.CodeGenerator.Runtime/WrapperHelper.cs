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

        private static readonly Dictionary < Type, Func < object, ABSObject > > s_WrapperMap =
            new Dictionary < Type, Func < object, ABSObject > >();

        #region Public

        public static void AddRecastWrapper( Type t, Func < object, ABSObject > f )
        {
            s_WrapperMap[t] = f;
        }

        public static void AddRecastWrapper < T >( Func < T, ABSObject > f )
        {
            AddRecastWrapper( typeof( T ), x => f( ( T )x ) );
        }

        public static ABSScriptInterface CreateInterface( this WrapperStaticDataBase db, string name )
        {
            return new BSConstantScriptInterface(
                                                 name,
                                                 table =>
                                                 {
                                                     foreach ( KeyValuePair < Type, BSStaticWrapperObject > dbType in
                                                         db.StaticTypes )
                                                     {
                                                         ABSTable t =
                                                             table.HasElement( new BSObject( dbType.Key.Name ) )
                                                                 ? ( ABSTable )table.
                                                                               GetRawElement(
                                                                                    new BSObject( dbType.Key.Name )
                                                                                   ).
                                                                               ResolveReference()
                                                                 : new BSTable( SourcePosition.Unknown );

                                                         table.InsertElement( new BSObject( dbType.Key.Name ), t );

                                                         foreach ( string valueProperty in dbType.Value.Properties )
                                                         {
                                                             t.InsertElement(
                                                                             new BSObject( valueProperty ),
                                                                             dbType.Value.GetProperty( valueProperty )
                                                                            );
                                                         }
                                                     }
                                                 }
                                                );
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
                                                                          ? ( ABSTable )table.
                                                                              GetRawElement(
                                                                                   new BSObject( dbType.Name )
                                                                                  ).
                                                                              ResolveReference()
                                                                          : new BSTable( SourcePosition.Unknown );

                                                         table.InsertElement( new BSObject( dbType.Name ), t );

                                                         t.InsertElement(
                                                                         new BSObject( dbType.Name ),
                                                                         new
                                                                             BSFunction(
                                                                                  $"function {dbType.Name}(args)",
                                                                                  objects => db.Get(
                                                                                       dbType,
                                                                                       objects.Select(
                                                                                                UnwrapObject < object >
                                                                                               ).
                                                                                           ToArray()
                                                                                      ),
                                                                                  0,
                                                                                  int.MaxValue
                                                                                 )
                                                                        );
                                                     }
                                                 }
                                                );
        }

        public static ABSObject RecastWrapper( ABSObject o, bool findBase = false )
        {
            if ( o is IBSWrappedObject wo )
            {
                object oInstance = wo.GetInternalObject();

                if ( oInstance == null )
                {
                    throw new BSRuntimeException( "Can not Recast Object that is NULL" );
                }

                Type t = oInstance.GetType();

                if ( s_WrapperMap.ContainsKey( t ) )
                {
                    return s_WrapperMap[t]( oInstance );
                }

                if ( !findBase )
                {
                    return o;
                }

                Type cur = t.BaseType;

                while ( cur != null )
                {
                    if ( s_WrapperMap.ContainsKey( cur ) )
                    {
                        return s_WrapperMap[cur]( oInstance );
                    }

                    cur = cur.BaseType;
                }

                return o;
            }

            throw new BSRuntimeException( "Invalid Type. Expected Wrapper Object for Recast" );
        }

        public static object UnwrapObject( Type t, ABSObject o )
        {
            o = o.ResolveReference();

            if ( o is IBSWrappedObject obj )
            {
                object oi = obj.GetInternalObject();

                if ( t.IsInstanceOfType( oi ) )
                {
                    return oi;
                }

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

            throw new BSRuntimeException( $"Can not Unwrap Object: {o}" );
        }

        public static T UnwrapObject < T >( ABSObject o )
        {
            return ( T )UnwrapObject( typeof( T ), o );
        }

        #endregion

    }

}

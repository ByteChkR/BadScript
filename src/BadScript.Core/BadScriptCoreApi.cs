using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;
using BadScript.Interfaces;

namespace BadScript.Core
{

    public class BadScriptCoreApi : ABSScriptInterface
    {
        #region Public

        public BadScriptCoreApi() : base( "core" )
        {
        }

        public override void AddApi( ABSTable root )
        {

            root.InsertElement(
                new BSObject( "size" ),
                new BSFunction(
                    "function size(table/array)",
                    ( args ) =>
                    {
                        ABSObject arg = args[0];

                        if ( arg is BSArrayReference
                            arRef )
                        {
                            arg = arRef.Get();
                        }

                        if ( arg is BSTableReference
                            tRef )
                        {
                            arg = tRef.Get();
                        }

                        if ( arg is ABSArray ar )
                        {
                            return new BSObject( ( decimal ) ar.GetLength() );
                        }

                        if ( arg is ABSTable t )
                        {
                            return new BSObject( ( decimal ) t.GetLength() );
                        }

                        throw new BSInvalidTypeException(
                            arg.Position,
                            "Can not get Size of object",
                            arg,
                            "Table",
                            "Array"
                        );
                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "keys" ),
                new BSFunction(
                    "function keys(table)",
                    objects =>
                    {
                        if ( objects[0] is ABSTable t )
                        {
                            return t.Keys;
                        }

                        throw new BSInvalidTypeException(
                            objects[0].Position,
                            "Object is not a table",
                            objects[0],
                            "Table"
                        );
                    },
                    1 ) );

            root.InsertElement(
                new BSObject( "values" ),
                new BSFunction(
                    "function values(table)",
                    objects =>
                    {
                        if ( objects[0] is ABSTable t )
                        {
                            return t.Keys;
                        }

                        throw new BSInvalidTypeException(
                            objects[0].Position,
                            "Object is not a table",
                            objects[0],
                            "Table"
                        );
                    },
                    1 ) );

            root.InsertElement(
                new BSObject( "error" ),
                new BSFunction(
                    "function error(obj)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        throw new BSRuntimeException( arg.Position, arg.ToString() );
                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "debug" ),
                new BSFunction(
                    "function debug(obj)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        return new BSObject( arg.SafeToString( new Dictionary < ABSObject, string >() ) );
                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "isArray" ),
                new BSFunction(
                    "function isArray(obj)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        if ( arg is ABSArray )
                        {
                            return BSObject.One;
                        }

                        return BSObject.Zero;

                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "lock" ),
                new BSFunction(
                    "function lock(array/table)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        if ( arg is BSArray arr )
                        {
                            arr.Lock();
                        }
                        else if ( arg is BSTable table )
                        {
                            table.Lock();
                        }

                        return BSObject.Null;

                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "hasKey" ),
                new BSFunction(
                    "function hasKey(table, key)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        if ( arg is ABSTable table )
                        {
                            return new BSObject(
                                ( decimal ) ( table.HasElement( args[1].ResolveReference() ) ? 1 : 0 ) );
                        }

                        throw new BSInvalidTypeException( SourcePosition.Unknown, "Expected Table", arg, "Table" );
                    },
                    2
                )
            );

            root.InsertElement(
                new BSObject( "isTable" ),
                new BSFunction(
                    "function isTable(obj)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        if ( arg is ABSTable )
                        {
                            return BSObject.One;
                        }

                        return BSObject.Zero;

                    },
                    1
                )
            );

            root.InsertElement( new BSObject( "escape" ), new BSFunction( "function escape(str)", EscapeString, 1 ) );

            root.InsertElement(
                new BSObject( "base64" ),
                new BSTable(
                    SourcePosition.Unknown,
                    new Dictionary < ABSObject, ABSObject >
                    {
                        { new BSObject( "to" ), new BSFunction( "function to(dataArray)", ToBase64, 1 ) },
                        { new BSObject( "from" ), new BSFunction( "function from(str)", FromBase64, 1 ) },
                    } ) );

            root.InsertElement(
                new BSObject( "sleep" ),
                new BSFunction(
                    "function sleep(ms)",
                    ( args ) =>
                    {
                        if ( args[0].TryConvertDecimal( out decimal lD ) )
                        {
                            Thread.Sleep( ( int ) lD );

                            return BSObject.Null;
                        }

                        throw new BSInvalidTypeException(
                            args[0].Position,
                            "Invalid Sleep Time",
                            args[0],
                            "number"
                        );
                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "hook" ),
                new BSFunction( "function hook(target, hook)", HookFunction, 2 ) );

            root.InsertElement(
                new BSObject( "releaseHook" ),
                new BSFunction( "function releaseHook(target, hook)", ReleaseHookFunction, 2 ) );

            root.InsertElement(
                new BSObject( "releaseHooks" ),
                new BSFunction( "function releaseHooks(target)", ReleaseHooksFunction, 1 ) );
        }

        #endregion

        #region Private

        private ABSObject EscapeString( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();

            return new BSObject( Uri.EscapeDataString( str ) );
        }

        private ABSObject FromBase64( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();
            byte[] data = Convert.FromBase64String( str );

            return new BSArray( data.Select( x => new BSObject( ( decimal ) x ) ) );
        }

        private ABSObject HookFunction( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is BSFunction target )
            {
                if ( arg[1].ResolveReference() is BSFunction hook )
                {
                    target.AddHook( hook );

                    return BSObject.Null;
                }

                throw new BSInvalidTypeException(
                    SourcePosition.Unknown,
                    "Expected Function as argument.",
                    arg[1],
                    "BSFunction" );
            }

            throw new BSInvalidTypeException(
                SourcePosition.Unknown,
                "Expected Function as argument.",
                arg[0],
                "BSFunction" );
        }

        private ABSObject ReleaseHookFunction( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is BSFunction target )
            {
                if ( arg[1].ResolveReference() is BSFunction hook )
                {
                    target.RemoveHook( hook );

                    return BSObject.Null;
                }

                throw new BSInvalidTypeException(
                    SourcePosition.Unknown,
                    "Expected Function as argument.",
                    arg[1],
                    "BSFunction" );
            }

            throw new BSInvalidTypeException(
                SourcePosition.Unknown,
                "Expected Function as argument.",
                arg[0],
                "BSFunction" );
        }

        private ABSObject ReleaseHooksFunction( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is BSFunction target )
            {
                target.ClearHooks();
            }

            throw new BSInvalidTypeException(
                SourcePosition.Unknown,
                "Expected Function as argument.",
                arg[0],
                "BSFunction" );
        }

        private ABSObject ToBase64( ABSObject[] arg )
        {
            if ( arg[0].ResolveReference() is ABSArray a )
            {
                byte[] data = new byte[a.GetLength()];

                for ( int i = 0; i < data.Length; i++ )
                {
                    data[i] = ( byte ) a.GetElement( i ).ConvertDecimal();
                }

                return new BSObject( Convert.ToBase64String( data ) );
            }

            throw new BSRuntimeException( "Invalid Type. Expected Array of Numbers" );
        }

        #endregion
    }

}

using System.Collections.Generic;
using System.Threading;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Core
{

    public static class BadScriptCoreApi
    {
        #region Public

        public static void AddApi()
        {
            BSEngine.AddStatic(
                "size",
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
                            "Can not get Size of object",
                            arg,
                            "Table",
                            "Array"
                        );
                    },
                    1
                )
            );

            BSEngine.AddStatic(
                "keys",
                new BSFunction(
                    "function keys(table)",
                    objects =>
                    {
                        if ( objects[0] is ABSTable t )
                        {
                            return t.Keys;
                        }

                        throw new BSInvalidTypeException(
                            "Object is not a table",
                            objects[0],
                            "Table"
                        );
                    },
                    1 ) );

            BSEngine.AddStatic(
                "values",
                new BSFunction(
                    "function values(table)",
                    objects =>
                    {
                        if ( objects[0] is ABSTable t )
                        {
                            return t.Keys;
                        }

                        throw new BSInvalidTypeException(
                            "Object is not a table",
                            objects[0],
                            "Table"
                        );
                    },
                    1 ) );

            BSEngine.AddStatic(
                "error",
                new BSFunction(
                    "function error(obj)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        throw new BSRuntimeException( arg.ToString() );
                    },
                    1
                )
            );

            BSEngine.AddStatic(
                "debug",
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

            BSEngine.AddStatic(
                "sleep",
                new BSFunction(
                    "function sleep(ms)",
                    ( args ) =>
                    {
                        if ( args[0].TryConvertDecimal( out decimal lD ) )
                        {
                            Thread.Sleep( ( int ) lD );

                            return new BSObject( null );
                        }

                        throw new BSInvalidTypeException(
                            "Invalid Sleep Time",
                            args[0],
                            "number"
                        );
                    },
                    1
                )
            );
        }

        #endregion
    }

}

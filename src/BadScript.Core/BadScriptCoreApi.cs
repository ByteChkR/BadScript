using System.Collections.Generic;
using System.Threading;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Core
{

    public class BadScriptCoreApi : ABSScriptInterface
    {
        #region Public

        public override void AddApi(ABSTable root)
        {

            root.InsertElement(new BSObject("size"),
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

            root.InsertElement(new BSObject("keys"),
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

            root.InsertElement(new BSObject("values"),
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

            root.InsertElement(new BSObject("error"),
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

            root.InsertElement(new BSObject("debug"),
                               new BSFunction(
                                   "function debug(obj)",
                                   (args) =>
                                   {

                                       ABSObject arg = args[0].ResolveReference();

                                       return new BSObject(arg.SafeToString(new Dictionary<ABSObject, string>()));
                                   },
                                   1
                               )
            );

            root.InsertElement(new BSObject("isArray"),
                               new BSFunction(
                                   "function isArray(obj)",
                                   (args) =>
                                   {

                                       ABSObject arg = args[0].ResolveReference();

                                       if (arg is ABSArray)
                                           return new BSObject((decimal)1);
                                       return new BSObject((decimal)0);

                                   },
                                   1
                               )
            );

            root.InsertElement(new BSObject("isTable"),
                               new BSFunction(
                                   "function isTable(obj)",
                                   (args) =>
                                   {

                                       ABSObject arg = args[0].ResolveReference();

                                       if ( arg is ABSTable)
                                           return new BSObject((decimal)1);
                                       return new BSObject((decimal)0);

                                   },
                                   1
                               )
            );



            root.InsertElement(new BSObject("sleep"),
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

        public BadScriptCoreApi() : base( "core" )
        {
        }
    }

}

using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.ConsoleUtils
{

    public class ConsoleApi
    {
        #region Public

        public static void AddApi()
        {

            BSEngine.AddStatic(
                "print",
                new BSFunction(
                    "function print(obj)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        Console.WriteLine( arg );

                        return new BSObject( null );
                    },
                    1
                )
            );

            BSEngine.AddStatic(
                "read",
                new BSFunction(
                    "function read()",
                    ( args ) => new BSObject( Console.ReadLine() ),
                    0
                )
            );

        }

        #endregion
    }

}

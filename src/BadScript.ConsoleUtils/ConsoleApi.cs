using System;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.ConsoleUtils
{

    public class ConsoleApi : ABSScriptInterface
    {
        #region Public

        public ConsoleApi() : base( "console" )
        {
        }

        public override void AddApi( ABSTable root )
        {

            root.InsertElement(
                new BSObject( "print" ),
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

            root.InsertElement(
                new BSObject( "write" ),
                new BSFunction(
                    "function write(obj)",
                    ( args ) =>
                    {

                        ABSObject arg = args[0].ResolveReference();

                        Console.Write( arg );

                        return new BSObject( null );
                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "read" ),
                new BSFunction(
                    "function read()",
                    ( args ) => new BSObject( Console.ReadLine() ),
                    0
                )
            );

            root.InsertElement(
                new BSObject( "clear" ),
                new BSFunction(
                    "function clear()",
                    ( args ) =>
                    {
                        Console.Clear();

                        return new BSObject( null );
                    },
                    0
                )
            );

        }

        #endregion
    }

}

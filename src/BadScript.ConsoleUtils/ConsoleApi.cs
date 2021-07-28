using System;
using BadScript.Common.Exceptions;
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

        #region Private

        private ABSObject GetConsoleBackColor( ABSObject[] arg )
        {
            return new BSObject( Console.BackgroundColor.ToString() );
        }

        private ABSObject GetConsoleForeColor( ABSObject[] arg )
        {
            return new BSObject( Console.ForegroundColor.ToString() );
        }

        private ABSObject SetConsoleBackColor( ABSObject[] arg )
        {
            if ( !Enum.TryParse( arg[0].ConvertString(), true, out ConsoleColor col ) )
            {
                throw new BSRuntimeException( "Could not create Console Color: " + arg[0].ConvertString() );
            }

            Console.BackgroundColor = col;

            return new BSObject( null );
        }

        private ABSObject SetConsoleForeColor( ABSObject[] arg )
        {
            if ( !Enum.TryParse( arg[0].ConvertString(), true, out ConsoleColor col ) )
            {
                throw new BSRuntimeException( "Could not create Console Color: " + arg[0].ConvertString() );
            }

            Console.ForegroundColor = col;

            return new BSObject( null );
        }

        #endregion
    }

}

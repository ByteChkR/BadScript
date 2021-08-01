using System;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.ConsoleUtils
{

    public class ConsoleApi : ABSScriptInterface
    {
        public event Action Clear;

        public event Action < ABSObject > Write;

        public event Action < ABSObject > WriteLine;

        public event Func < ABSObject > ReadLine;

        #region Public

        public ConsoleApi( bool addDefaultHandlers = true ) : base( "console" )
        {
            if ( addDefaultHandlers )
            {
                WriteLine = Console.WriteLine;
                ReadLine = () => new BSObject( Console.ReadLine() );
                Write = Console.Write;
                Clear = Console.Clear;
            }
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

                        WriteLine?.Invoke( arg );

                        return BSObject.Null;
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

                        Write?.Invoke( arg );

                        return BSObject.Null;
                    },
                    1
                )
            );

            root.InsertElement(
                new BSObject( "read" ),
                new BSFunction(
                    "function read()",
                    ( args ) => ReadLine?.Invoke() ?? BSObject.Null,
                    0
                )
            );

            root.InsertElement(
                new BSObject( "clear" ),
                new BSFunction(
                    "function clear()",
                    ( args ) =>
                    {
                        Clear?.Invoke();

                        return BSObject.Null;
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

            return BSObject.Null;
        }

        private ABSObject SetConsoleForeColor( ABSObject[] arg )
        {
            if ( !Enum.TryParse( arg[0].ConvertString(), true, out ConsoleColor col ) )
            {
                throw new BSRuntimeException( "Could not create Console Color: " + arg[0].ConvertString() );
            }

            Console.ForegroundColor = col;

            return BSObject.Null;
        }

        #endregion
    }

}

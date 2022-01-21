using System;

using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Reflection;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.ConsoleUtils
{

    public class BSSystemConsoleInterface : BSConsoleInterface
    {

        #region Public

        public BSSystemConsoleInterface()
        {
        }

        public override void AddApi( ABSTable root )
        {
            base.AddApi( root );
            root.SetRawElement( "Colors", EnumBuilder < ConsoleColor >.Build() );
            root.SetRawElement( "Keys", EnumBuilder < ConsoleKey >.Build() );
            root.SetRawElement( "Modifiers", EnumBuilder < ConsoleModifiers >.Build() );

            root.SetRawElement(
                               "ForeColor",
                               new BSReflectionReference( GetConsoleForeColor, SetConsoleForeColor )
                              );

            root.SetRawElement(
                               "BackColor",
                               new BSReflectionReference( GetConsoleBackColor, SetConsoleBackColor )
                              );

            root.SetRawElement(
                               "BufferWidth",
                               new BSReflectionReference(
                                                         GetConsoleBufferWidth,
                                                         SetConsoleBufferWidth
                                                        )
                              );

            root.SetRawElement(
                               "BufferHeight",
                               new BSReflectionReference(
                                                         GetConsoleBufferHeight,
                                                         SetConsoleBufferHeight
                                                        )
                              );

            root.SetRawElement(
                               "WindowWidth",
                               new BSReflectionReference(
                                                         GetConsoleWindowWidth,
                                                         SetConsoleWindowWidth
                                                        )
                              );

            root.SetRawElement(
                               "WindowHeight",
                               new BSReflectionReference(
                                                         GetConsoleWindowHeight,
                                                         SetConsoleWindowHeight
                                                        )
                              );

            root.SetRawElement(
                               "LargestWindowWidth",
                               new BSReflectionReference(
                                                         GetConsoleLargestWindowWidth,
                                                         null
                                                        )
                              );

            root.SetRawElement(
                               "LargestWindowHeight",
                               new BSReflectionReference(
                                                         GetConsoleLargestWindowHeight,
                                                         null
                                                        )
                              );

            root.SetRawElement(
                               "CursorSize",
                               new BSReflectionReference(
                                                         GetConsoleCursorSize,
                                                         SetConsoleCursorSize
                                                        )
                              );

            root.SetRawElement(
                               "CursorLeft",
                               new BSReflectionReference(
                                                         GetConsoleCursorLeft,
                                                         SetConsoleCursorLeft
                                                        )
                              );

            root.SetRawElement(
                               "CursorTop",
                               new BSReflectionReference(
                                                         GetConsoleCursorTop,
                                                         SetConsoleCursorTop
                                                        )
                              );

            root.SetRawElement(
                               "WindowLeft",
                               new BSReflectionReference(
                                                         GetConsoleWindowLeft,
                                                         SetConsoleWindowLeft
                                                        )
                              );

            root.SetRawElement(
                               "WindowTop",
                               new BSReflectionReference(
                                                         GetConsoleWindowTop,
                                                         SetConsoleWindowTop
                                                        )
                              );

            root.SetRawElement(
                               "Title",
                               new BSReflectionReference(
                                                         () => new BSObject( Console.Title ),
                                                         x => Console.Title = x.ConvertString(),
                                                         BSObject.EmptyString
                                                        )
                              );

            root.SetRawElement(
                               "CapsLock",
                               new BSReflectionReference(
                                                         () => Console.CapsLock ? BSObject.True : BSObject.False,
                                                         null,
                                                         BSObject.False
                                                        )
                              );

            root.SetRawElement(
                               "NumberLock",
                               new BSReflectionReference(
                                                         () => Console.NumberLock ? BSObject.True : BSObject.False,
                                                         null,
                                                         BSObject.False
                                                        )
                              );

            root.SetRawElement(
                               "IsErrorRedirected",
                               new BSReflectionReference(
                                                         () => Console.IsErrorRedirected
                                                                   ? BSObject.True
                                                                   : BSObject.False,
                                                         null,
                                                         BSObject.False
                                                        )
                              );

            root.SetRawElement(
                               "IsInputRedirected",
                               new BSReflectionReference(
                                                         () => Console.IsInputRedirected
                                                                   ? BSObject.True
                                                                   : BSObject.False,
                                                         null,
                                                         BSObject.False
                                                        )
                              );

            root.SetRawElement(
                               "IsOutputRedirected",
                               new BSReflectionReference(
                                                         () => Console.IsOutputRedirected
                                                                   ? BSObject.True
                                                                   : BSObject.False,
                                                         null,
                                                         BSObject.False
                                                        )
                              );

            root.InsertElement(
                               "IsKeyAvailable",
                               new BSFunction(
                                              "function IsKeyAvailable()",
                                              args => Console.KeyAvailable ? BSObject.True : BSObject.False,
                                              0
                                             )
                              );

            root.SetRawElement(
                               "TreatControlCAsInput",
                               new BSReflectionReference(
                                                         () => Console.TreatControlCAsInput
                                                                   ? BSObject.True
                                                                   : BSObject.False,
                                                         null,
                                                         BSObject.False
                                                        )
                              );

            root.SetRawElement(
                               "CursorVisible",
                               new BSReflectionReference(
                                                         () => Console.CursorVisible
                                                                   ? BSObject.True
                                                                   : BSObject.False,
                                                         null,
                                                         BSObject.False
                                                        )
                              );

            root.SetRawElement(
                               "Beep",
                               new BSFunction(
                                              "function Beep()",
                                              o =>
                                              {
                                                  Console.Beep();

                                                  return BSObject.Null;
                                              },
                                              0
                                             )
                              );

            root.SetRawElement(
                               "Read",
                               new BSFunction(
                                              "function Read()",
                                              o =>
                                              {
                                                  int c = Console.Read();

                                                  return new BSObject( ( decimal )c );
                                              },
                                              0
                                             )
                              );

            root.SetRawElement(
                               "ReadChar",
                               new BSFunction(
                                              "function ReadChar()",
                                              o =>
                                              {
                                                  int c = Console.Read();

                                                  if ( c == -1 )
                                                  {
                                                      c = '\0';
                                                  }

                                                  return new BSObject( new string( ( char )c, 1 ) );
                                              },
                                              0
                                             )
                              );

            root.SetRawElement(
                               "ResetColor",
                               new BSFunction(
                                              "function ResetColor()",
                                              o =>
                                              {
                                                  Console.ResetColor();

                                                  return BSObject.Null;
                                              },
                                              0
                                             )
                              );

            root.SetRawElement(
                               "ReadKey",
                               new BSFunction(
                                              "function ReadKey(intercept)",
                                              ConsoleReadKey,
                                              0
                                             )
                              );
        }

        #endregion

        #region Private

        private ABSObject ConsoleReadKey( ABSObject[] arg )
        {
            ConsoleKeyInfo i = arg.Length == 0 ? Console.ReadKey() : Console.ReadKey( arg[0].ConvertBool() );
            BSTable t = new BSTable( SourcePosition.Unknown );
            t.InsertElement( "Key", new BSObject( ( decimal )i.Key ) );
            t.InsertElement( "KeyChar", new BSObject( i.KeyChar.ToString() ) );
            t.InsertElement( "Modifiers", new BSObject( ( decimal )i.Modifiers ) );
            t.Lock();

            return t;
        }

        private ABSObject GetConsoleBackColor()
        {
            return new BSObject( ( decimal )Console.BackgroundColor );
        }

        private ABSObject GetConsoleBufferHeight()
        {
            return new BSObject( ( decimal )Console.BufferHeight );
        }

        private ABSObject GetConsoleBufferWidth()
        {
            return new BSObject( ( decimal )Console.BufferWidth );
        }

        private ABSObject GetConsoleCursorLeft()
        {
            return new BSObject( ( decimal )Console.CursorLeft );
        }

        private ABSObject GetConsoleCursorSize()
        {
            return new BSObject( ( decimal )Console.CursorSize );
        }

        private ABSObject GetConsoleCursorTop()
        {
            return new BSObject( ( decimal )Console.CursorTop );
        }

        private ABSObject GetConsoleForeColor()
        {
            return new BSObject( ( decimal )Console.ForegroundColor );
        }

        private ABSObject GetConsoleLargestWindowHeight()
        {
            return new BSObject( ( decimal )Console.LargestWindowHeight );
        }

        private ABSObject GetConsoleLargestWindowWidth()
        {
            return new BSObject( ( decimal )Console.LargestWindowWidth );
        }

        private ABSObject GetConsoleWindowHeight()
        {
            return new BSObject( ( decimal )Console.BufferHeight );
        }

        private ABSObject GetConsoleWindowLeft()
        {
            return new BSObject( ( decimal )Console.WindowLeft );
        }

        private ABSObject GetConsoleWindowTop()
        {
            return new BSObject( ( decimal )Console.WindowTop );
        }

        private ABSObject GetConsoleWindowWidth()
        {
            return new BSObject( ( decimal )Console.BufferWidth );
        }

        private void SetConsoleBackColor( ABSObject arg )
        {
            Console.BackgroundColor =
                ( ConsoleColor )Enum.ToObject( typeof( ConsoleColor ), ( int )arg.ConvertDecimal() );
        }

        private void SetConsoleBufferHeight( ABSObject o )
        {
            Console.BufferHeight = ( int )o.ConvertDecimal();
        }

        private void SetConsoleBufferWidth( ABSObject o )
        {
            Console.BufferWidth = ( int )o.ConvertDecimal();
        }

        private void SetConsoleCursorLeft( ABSObject o )
        {
            Console.CursorLeft = ( int )o.ConvertDecimal();
        }

        private void SetConsoleCursorSize( ABSObject o )
        {
            Console.CursorSize = ( int )o.ConvertDecimal();
        }

        private void SetConsoleCursorTop( ABSObject o )
        {
            Console.CursorTop = ( int )o.ConvertDecimal();
        }

        private void SetConsoleForeColor( ABSObject arg )
        {
            Console.ForegroundColor =
                ( ConsoleColor )Enum.ToObject( typeof( ConsoleColor ), ( int )arg.ConvertDecimal() );
        }

        private void SetConsoleWindowHeight( ABSObject o )
        {
            Console.BufferHeight = ( int )o.ConvertDecimal();
        }

        private void SetConsoleWindowLeft( ABSObject o )
        {
            Console.WindowLeft = ( int )o.ConvertDecimal();
        }

        private void SetConsoleWindowTop( ABSObject o )
        {
            Console.WindowTop = ( int )o.ConvertDecimal();
        }

        private void SetConsoleWindowWidth( ABSObject o )
        {
            Console.BufferWidth = ( int )o.ConvertDecimal();
        }

        #endregion

    }

}

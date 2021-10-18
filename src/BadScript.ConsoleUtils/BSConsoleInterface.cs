using System;

using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.ConsoleUtils
{

    public class BSConsoleInterface : ABSScriptInterface
    {

        public event Action Clear;

        public event Action < ABSObject > Write;

        public event Action < ABSObject > WriteLine;

        public event Func < ABSObject > ReadLine;

        #region Public

        public BSConsoleInterface(
            Action < ABSObject > write,
            Action < ABSObject > writeLine,
            Action clear,
            Func < ABSObject > read ) : this( false )
        {
            Clear = clear;
            Write = write;
            WriteLine = writeLine;
            ReadLine = read;
        }

        public BSConsoleInterface( bool addDefaultHandlers = true ) : base( "Console" )
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
                               new BSObject( "WriteLine" ),
                               new BSFunction(
                                              "function WriteLine(obj)",
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
                               new BSObject( "Write" ),
                               new BSFunction(
                                              "function Write(obj)",
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
                               new BSObject( "ReadLine" ),
                               new BSFunction(
                                              "function ReadLine()",
                                              ( args ) => ReadLine?.Invoke() ?? BSObject.Null,
                                              0
                                             )
                              );
            

            root.InsertElement(
                               new BSObject( "Clear" ),
                               new BSFunction(
                                              "function ReadLine()",
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

    }

}

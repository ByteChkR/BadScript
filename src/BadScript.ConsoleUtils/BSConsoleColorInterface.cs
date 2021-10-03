using System;

using BadScript.Exceptions;
using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.ConsoleUtils
{

    public class BSConsoleColorInterface : ABSScriptInterface
    {

        #region Public

        public BSConsoleColorInterface() : base( "console-colors" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(
                               new BSObject( "setForeColor" ),
                               new BSFunction( "function setForeColor(consoleColor)", SetConsoleForeColor, 1 )
                              );

            root.InsertElement(
                               new BSObject( "setBackColor" ),
                               new BSFunction( "function setBackColor(consoleColor)", SetConsoleBackColor, 1 )
                              );

            root.InsertElement(
                               new BSObject( "getForeColor" ),
                               new BSFunction( "function getForeColor()", GetConsoleForeColor, 0 )
                              );

            root.InsertElement(
                               new BSObject( "getBackColor" ),
                               new BSFunction( "function getBackColor()", GetConsoleBackColor, 0 )
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

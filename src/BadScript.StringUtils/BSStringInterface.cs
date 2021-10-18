using System;
using System.Linq;
using System.Text.RegularExpressions;

using BadScript.Exceptions;
using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.StringUtils
{

    public class BSStringInterface : ABSScriptInterface
    {

        #region Public

        public BSStringInterface() : base( "String" )
        {
        }

        public override void AddApi( ABSTable apiRoot )
        {
            apiRoot.InsertElement(
                                  new BSObject( "Escape" ),
                                  new BSFunction( "function Escape(str)", EscapeString, 1 )
                                 );

            apiRoot.InsertElement(
                                  new BSObject( "RegexEscape" ),
                                  new BSFunction( "function RegexEscape(str)", RegexEscapeString, 1 )
                                 );

            apiRoot.InsertElement(
                                  new BSObject( "RegexUnescape" ),
                                  new BSFunction( "function RegexUnescape(str)", RegexUnescapeString, 1 )
                                 );

            apiRoot.InsertElement(
                                  "Trim",
                                  MakeFunction(
                                               "function Trim(str)/",
                                               x => new BSObject( x[0].ConvertString().Trim() ),
                                               1,
                                               1
                                              )
                                 );

            apiRoot.InsertElement(
                                  "TrimEnd",
                                  MakeFunction(
                                               "function TrimEnd(str)/",
                                               x => new BSObject( x[0].ConvertString().TrimEnd() ),
                                               1,
                                               1
                                              )
                                 );

            apiRoot.InsertElement(
                                  "TrimStart",
                                  MakeFunction(
                                               "function TrimStart(str)/",
                                               x => new BSObject( x[0].ConvertString().TrimEnd() ),
                                               1,
                                               1
                                              )
                                 );

            apiRoot.InsertElement(
                                  "Split",
                                  MakeFunction(
                                               "function Split(str, split0, split1, split2, ...)",
                                               Split,
                                               1,
                                               int.MaxValue
                                              )
                                 );

            apiRoot.InsertElement(
                                  "Substr",
                                  MakeFunction( "function Substr(str, start)/substr(str, start, length)", Substr, 2, 3 )
                                 );

            //charAt(str, index)
            apiRoot.InsertElement(
                                  "CharAt",
                                  MakeFunction( "function CharAt(str, index)", CharAt, 2, 2 )
                                 );

            //end/startsWith(str, start)
            apiRoot.InsertElement(
                                  "EndsWith",
                                  MakeFunction( "function EndsWith(str, end)", EndsWith, 2, 2 )
                                 );

            apiRoot.InsertElement(
                                  "StartsWith",
                                  MakeFunction( "function StartsWith(str, start)", StartsWith, 2, 2 )
                                 );

            //indexOf(str, searchStr)/indexOf(str, searchStr, start)
            apiRoot.InsertElement(
                                  "IndexOf",
                                  MakeFunction( "function IndexOf(str, searchStr)", IndexOf, 2, 2 )
                                 );

            //insert(str, 
            apiRoot.InsertElement(
                                  "Insert",
                                  MakeFunction( "function Insert(str, index, s)", Insert, 3, 3 )
                                 );

            //lastIndexOf(str, searchStr)/lastIndexOf(str, searchStr, start)
            apiRoot.InsertElement(
                                  "LastIndexOf",
                                  MakeFunction( "function LastIndexOf(str, searchStr)", LastIndexOf, 2, 2 )
                                 );

            //remove
            apiRoot.InsertElement(
                                  "Remove",
                                  MakeFunction( "function Remove(str, start, length)", Remove, 3, 3 )
                                 );

            //replace
            apiRoot.InsertElement(
                                  "Replace",
                                  MakeFunction( "function Replace(str, old, new)", Replace, 3, 3 )
                                 );

            //toArray
            apiRoot.InsertElement(
                                  "ToArray",
                                  MakeFunction( "function ToArray(str)", ToArray, 1, 1 )
                                 );

            //toUpper
            apiRoot.InsertElement(
                                  "ToUpper",
                                  MakeFunction( "function ToUpper(str)", ToUpper, 1, 1 )
                                 );

            apiRoot.InsertElement(
                                  "Length",
                                  MakeFunction( "function Length(str)", StringGetLength, 1, 1 )
                                 );

            apiRoot.InsertElement(
                                  "IsWhiteSpace",
                                  MakeFunction( "function IsWhiteSpace(str)", StringIsWhiteSpace, 1, 1 )
                                 );

            apiRoot.InsertElement(
                                  "IsLetter",
                                  MakeFunction( "function IsLetter(str)", StringIsLetter, 1, 1 )
                                 );

            apiRoot.InsertElement(
                                  "IsDigit",
                                  MakeFunction( "function IsDigit(str)", StringIsDigit, 1, 1 )
                                 );

            //toLower

            apiRoot.InsertElement(
                                  "ToLower",
                                  MakeFunction( "function ToLower(str)", ToLower, 1, 1 )
                                 );

            apiRoot.InsertElement(
                                  "Format",
                                  new BSFunction(
                                                 "function Format(formatStr, arg0, arg1, ...)",
                                                 args =>
                                                 {
                                                     ABSObject format = args[0].ResolveReference();

                                                     if ( format.TryConvertString( out string formatStr ) )
                                                     {
                                                         return new BSObject(
                                                                             string.Format(
                                                                                  formatStr,
                                                                                  args.Skip( 1 ).
                                                                                      Cast < object >().
                                                                                      ToArray()
                                                                                 )
                                                                            );
                                                     }

                                                     throw new BSInvalidTypeException(
                                                          format.Position,
                                                          "Invalid Format string type",
                                                          format,
                                                          "string"
                                                         );
                                                 },
                                                 1,
                                                 int.MaxValue
                                                )
                                 );
        }

        #endregion

        #region Private

        private static ABSObject CharAt( ABSObject[] arg )
        {
            return new BSObject( arg[0].ConvertString()[( int )arg[1].ConvertDecimal()].ToString() );
        }

        private static ABSObject EndsWith( ABSObject[] arg )
        {
            return arg[0].ConvertString().EndsWith( arg[1].ConvertString() ) ? BSObject.True : BSObject.False;
        }

        private static ABSObject IndexOf( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();
            string searchStr = arg[1].ConvertString();
            int idx = str.IndexOf( searchStr );

            return new BSObject( ( decimal )idx );
        }

        private static ABSObject Insert( ABSObject[] arg )
        {
            return new BSObject(
                                arg[0].ConvertString().Insert( ( int )arg[1].ConvertDecimal(), arg[2].ConvertString() )
                               );
        }

        private static ABSObject LastIndexOf( ABSObject[] arg )
        {
            return new BSObject( ( decimal )arg[0].ConvertString().LastIndexOf( arg[1].ConvertString() ) );
        }

        private static BSFunction MakeFunction( string data, Func < ABSObject[], ABSObject > f, int min, int max )
        {
            return new BSFunction(
                                  data,
                                  f,
                                  min,
                                  max
                                 );
        }

        private static ABSObject Remove( ABSObject[] arg )
        {
            return new BSObject(
                                arg[0].
                                    ConvertString().
                                    Remove( ( int )arg[1].ConvertDecimal(), ( int )arg[2].ConvertDecimal() )
                               );
        }

        private static ABSObject Replace( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();
            string oldS = arg[1].ConvertString();
            string newS = arg[2].ConvertString();

            return new BSObject( str.Replace( oldS, newS ) );
        }

        private static ABSObject Split( ABSObject[] arg )
        {
            ABSObject str = arg[0].ResolveReference();

            return new BSArray(
                               str.ConvertString().
                                   Split(
                                         arg.Skip( 1 ).Select( x => x.ConvertString() ).ToArray(),
                                         StringSplitOptions.None
                                        ).
                                   Select( x => new BSObject( x ) )
                              );
        }

        private static ABSObject StartsWith( ABSObject[] arg )
        {
            return arg[0].ConvertString().StartsWith( arg[1].ConvertString() ) ? BSObject.True : BSObject.False;
        }

        private static ABSObject Substr( ABSObject[] arg )
        {
            if ( arg.Length == 2 )
            {
                return new BSObject( arg[0].ConvertString().Substring( ( int )arg[1].ConvertDecimal() ) );
            }

            return new BSObject(
                                arg[0].
                                    ConvertString().
                                    Substring( ( int )arg[1].ConvertDecimal(), ( int )arg[2].ConvertDecimal() )
                               );
        }

        private static ABSObject ToArray( ABSObject[] arg )
        {
            return new BSArray( arg[0].ConvertString().ToCharArray().Select( x => new BSObject( x.ToString() ) ) );
        }

        private static ABSObject ToLower( ABSObject[] arg )
        {
            return new BSObject( arg[0].ConvertString().ToLower() );
        }

        private static ABSObject ToUpper( ABSObject[] arg )
        {
            return new BSObject( arg[0].ConvertString().ToUpper() );
        }

        private ABSObject EscapeString( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();

            return new BSObject( Uri.EscapeDataString( str ) );
        }

        private ABSObject RegexEscapeString( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();

            return new BSObject( Regex.Escape( str ) );
        }

        private ABSObject RegexUnescapeString( ABSObject[] arg )
        {
            string str = arg[0].ConvertString();

            return new BSObject( Regex.Unescape( str ) );
        }

        private ABSObject StringGetLength( ABSObject[] arg )
        {
            return new BSObject( ( decimal )arg[0].ConvertString().Length );
        }

        private ABSObject StringIsDigit( ABSObject[] arg )
        {
            return char.IsDigit( arg[0].ConvertString()[0] ) ? BSObject.True : BSObject.False;
        }

        private ABSObject StringIsLetter( ABSObject[] arg )
        {
            return char.IsLetter( arg[0].ConvertString()[0] ) ? BSObject.True : BSObject.False;
        }

        private ABSObject StringIsWhiteSpace( ABSObject[] arg )
        {
            return string.IsNullOrWhiteSpace( arg[0].ConvertString() ) ? BSObject.True : BSObject.False;
        }

        #endregion

    }

}

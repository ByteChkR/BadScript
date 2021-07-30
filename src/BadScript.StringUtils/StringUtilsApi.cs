using System;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.StringUtils
{

    public class StringUtilsApi : ABSScriptInterface
    {
        #region Public

        public StringUtilsApi() : base( "string" )
        {
        }

        public override void AddApi( ABSTable apiRoot )
        {

            apiRoot.InsertElement(
                "toNumber",
                MakeFunction(
                    "function toNumber(str)/",
                    x => new BSObject( decimal.Parse( x[0].ConvertString().Trim() ) ),
                    1,
                    1 ) );

            apiRoot.InsertElement(
                "trim",
                MakeFunction(
                    "function trim(str)/",
                    x => new BSObject( x[0].ConvertString().Trim() ),
                    1,
                    1 ) );

            apiRoot.InsertElement(
                "trimEnd",
                MakeFunction(
                    "function trimEnd(str)/",
                    x => new BSObject( x[0].ConvertString().TrimEnd() ),
                    1,
                    1 ) );

            apiRoot.InsertElement(
                "trimStart",
                MakeFunction(
                    "function trimStart(str)/",
                    x => new BSObject( x[0].ConvertString().TrimEnd() ),
                    1,
                    1 ) );

            apiRoot.InsertElement(
                "split",
                MakeFunction( "function split(str, split0, split1, split2, ...)", Split, 1, int.MaxValue ) );

            apiRoot.InsertElement(
                "substr",
                MakeFunction( "function substr(str, start)/substr(str, start, length)", Substr, 2, 3 ) );

            //charAt(str, index)
            apiRoot.InsertElement(
                "charAt",
                MakeFunction( "function charAt(str, index)", CharAt, 2, 2 ) );

            //end/startsWith(str, start)
            apiRoot.InsertElement(
                "endsWith",
                MakeFunction( "function endsWith(str, end)", EndsWith, 2, 2 ) );

            apiRoot.InsertElement(
                "startsWith",
                MakeFunction( "function startsWith(str, start)", StartsWith, 2, 2 ) );

            //indexOf(str, searchStr)/indexOf(str, searchStr, start)
            apiRoot.InsertElement(
                "indexOf",
                MakeFunction( "function indexOf(str, searchStr)", IndexOf, 2, 2 ) );

            //insert(str, 
            apiRoot.InsertElement(
                "insert",
                MakeFunction( "function insert(str, index, s)", Insert, 3, 3 ) );

            //lastIndexOf(str, searchStr)/lastIndexOf(str, searchStr, start)
            apiRoot.InsertElement(
                "lastIndexOf",
                MakeFunction( "function lastIndexOf(str, searchStr)", LastIndexOf, 2, 2 ) );

            //remove
            apiRoot.InsertElement(
                "remove",
                MakeFunction( "function remove(str, start, length)", Remove, 3, 3 ) );

            //replace
            apiRoot.InsertElement(
                "replace",
                MakeFunction( "function replace(str, old, new)", Replace, 3, 3 ) );

            //toArray
            apiRoot.InsertElement(
                "toArray",
                MakeFunction( "function toArray(str)", ToArray, 1, 1 ) );

            //toUpper
            apiRoot.InsertElement(
                "toUpper",
                MakeFunction( "function toUpper(str)", ToUpper, 1, 1 ) );

            apiRoot.InsertElement(
                "length",
                MakeFunction( "function length(str)", StringGetLength, 1, 1 ) );

            apiRoot.InsertElement(
                "isWhiteSpace",
                MakeFunction( "function isWhiteSpace(str)", StringIsWhiteSpace, 1, 1 ) );

            apiRoot.InsertElement(
                "isLetter",
                MakeFunction( "function isLetter(str)", StringIsLetter, 1, 1 ) );

            apiRoot.InsertElement(
                "isDigit",
                MakeFunction( "function isDigit(str)", StringIsDigit, 1, 1 ) );

            //toLower

            apiRoot.InsertElement(
                "toLower",
                MakeFunction( "function toLower(str)", ToLower, 1, 1 ) );

            apiRoot.InsertElement(
                "format",
                new BSFunction(
                    "function format(formatStr, arg0, arg1, ...)",
                    args =>
                    {
                        ABSObject format = args[0].ResolveReference();

                        if ( format.TryConvertString( out string formatStr ) )
                        {
                            return new BSObject(
                                string.Format(
                                    formatStr,
                                    args.Skip( 1 ).Cast < object >().ToArray()
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
            return new BSObject( arg[0].ConvertString()[( int ) arg[1].ConvertDecimal()].ToString() );
        }

        private static ABSObject EndsWith( ABSObject[] arg )
        {
            return new BSObject( ( decimal ) ( arg[0].ConvertString().EndsWith( arg[1].ConvertString() ) ? 1 : 0 ) );
        }

        private static ABSObject IndexOf( ABSObject[] arg )
        {
            string test = "HELLO WORLD\r\nHELLO WORLD\r\n";
            int testI = test.IndexOf( "\n" );
            string str = arg[0].ConvertString();
            string searchStr = arg[1].ConvertString();
            int idx = str.IndexOf( searchStr );

            return new BSObject( ( decimal ) idx );
        }

        private static ABSObject Insert( ABSObject[] arg )
        {
            return new BSObject(
                arg[0].ConvertString().Insert( ( int ) arg[1].ConvertDecimal(), arg[2].ConvertString() ) );
        }

        private static ABSObject LastIndexOf( ABSObject[] arg )
        {
            return new BSObject( ( decimal ) arg[0].ConvertString().LastIndexOf( arg[1].ConvertString() ) );
        }

        private static BSFunction MakeFunction( string data, Func < ABSObject[], ABSObject > f, int min, int max )
        {
            return new BSFunction(
                data,
                f,
                min,
                max );
        }

        private static ABSObject Remove( ABSObject[] arg )
        {
            return new BSObject(
                arg[0].ConvertString().Remove( ( int ) arg[1].ConvertDecimal(), ( int ) arg[2].ConvertDecimal() ) );
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
                    Split( arg.Skip( 1 ).Select( x => x.ConvertString() ).ToArray(), StringSplitOptions.None ).
                    Select( x => new BSObject( x ) ) );
        }

        private static ABSObject StartsWith( ABSObject[] arg )
        {
            return new BSObject( ( decimal ) ( arg[0].ConvertString().StartsWith( arg[1].ConvertString() ) ? 1 : 0 ) );
        }

        private static ABSObject Substr( ABSObject[] arg )
        {
            if ( arg.Length == 2 )
            {
                return new BSObject( arg[0].ConvertString().Substring( ( int ) arg[1].ConvertDecimal() ) );
            }

            return new BSObject(
                arg[0].ConvertString().Substring( ( int ) arg[1].ConvertDecimal(), ( int ) arg[2].ConvertDecimal() ) );

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

        private ABSObject StringGetLength( ABSObject[] arg )
        {
            return new BSObject( ( decimal ) arg[0].ConvertString().Length );
        }

        private ABSObject StringIsDigit( ABSObject[] arg )
        {
            return new BSObject( ( decimal ) ( char.IsDigit( arg[0].ConvertString()[0] ) ? 1 : 0 ) );
        }

        private ABSObject StringIsLetter( ABSObject[] arg )
        {
            return new BSObject( ( decimal ) ( char.IsLetter( arg[0].ConvertString()[0] ) ? 1 : 0 ) );
        }

        private ABSObject StringIsWhiteSpace( ABSObject[] arg )
        {
            return new BSObject( ( decimal ) ( string.IsNullOrWhiteSpace( arg[0].ConvertString() ) ? 1 : 0 ) );
        }

        #endregion
    }

}

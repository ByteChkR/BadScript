using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using BadScript.Exceptions;
using BadScript.Parser;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Types;

namespace BadScript.Testing
{

    public static class Program
    {
        
        #region Public

        private static BSExpression Parse(string str)
        {
            BSParser p = new BSParser(str );

            return p.ParseString( true );
        }

        public static void Main( string[] args )
        {
            string[] src =
            {
                "\"LoadInterface Function: {Environment.Debug(Environment.LoadInterface)}\"",
                "\"Masked Character: {{\"",
                "\"Masked Character: }}\"",
                "\"Masked Character: {{ABC}}\"",
                "\"{Environment.Debug(Environment.LoadInterface)} LoadInterface Function\"",
                "\"{{Masked Character\"",
                "\"}}Masked Character\"",
                "\"{{ABC}} Masked Character\"",
                "\"LoadInterface {Environment.Debug(Environment.LoadInterface)} Function\"",
                "\"Masked {{ Character\"",
                "\"Masked }} Character\"",
                "\"Masked {{ABC}} Character\"",
            };

            BSEngine e = BSEngineSettings.MakeDefault().Build();
            foreach ( string s in src )
            {
                BSExpression expr = Parse( s );
                BSReturnExpression r = new BSReturnExpression( SourcePosition.Unknown, expr );
                ABSObject o = e.LoadScript( new[] { r } );
                System.Console.WriteLine( o.ToString() );
            }

        }

        #endregion

    }

}

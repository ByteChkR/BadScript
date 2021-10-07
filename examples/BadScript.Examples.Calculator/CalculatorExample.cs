using System;

using BadScript.Parser.Expressions;
using BadScript.Scopes;
using BadScript.Types;

namespace BadScript.Examples.Calculator
{

    internal class CalculatorExample
    {

        #region Private

        private static BSEngine CreateEngine()
        {
            Console.WriteLine( "Creating Script Engine" );
            BSEngineSettings settings = BSEngineSettings.MakeDefault();

            return settings.Build();
        }

        private static void Main( string[] args )
        {
            BSEngine engine = CreateEngine();

            BSScope scope = new BSScope( engine ); //Create a Scope that we can use to run our expression in

            while ( true )
            {
                Console.Write( "Enter Expression to Calculate: " );
                string expr = Console.ReadLine();

                try
                {
                    //Parse the Input
                    BSExpression[] expressions = engine.ParseString( expr );

                    //Make sure that we only got one expression
                    if ( expressions.Length != 1 )
                    {
                        Console.WriteLine( "Invalid Input. Got multiple Expressions" );

                        continue;
                    }

                    //Execute the expression inside the scope we created
                    ABSObject ret = expressions[0].Execute( scope );

                    Console.WriteLine( $"Output: {expr} = {ret}" );
                }
                catch ( Exception e )
                {
                    Console.WriteLine( $"({e.GetType().Name})Input Error: {expr}" );
                }
            }
        }

        #endregion

    }

}

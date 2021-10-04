using System;
using System.Collections;
using System.Collections.Generic;

using BadScript.ConsoleUtils;
using BadScript.Parser;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Parser.OperatorImplementations;
using BadScript.Parser.Operators;
using BadScript.Parser.Operators.Implementations;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Examples.SimpleCustomExpression
{

    public class BSRangeEnumerableObject : ABSObject, IEnumerable <IForEachIteration>
    {

        private readonly int m_Start;
        private readonly int m_End;
        public BSRangeEnumerableObject( SourcePosition pos,int start, int end) : base( pos )
        {
            m_Start =start;
            m_End = end;

        }

        public override bool IsNull => false;

        public override bool Equals( ABSObject other )
        {
            return other is BSRangeEnumerableObject o && ReferenceEquals(this, o);
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new NotSupportedException();
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new NotSupportedException();
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return "Range Enumerator";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new NotSupportedException();
        }

        public override bool TryConvertBool( out bool v )
        {
            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = null;

            return false;
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            for ( int i = m_Start; i < m_End; i++ )
            {
                yield return new ForEachIteration( new BSObject( ( decimal )i ) );
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }



    public class BSRangeCustomOperatorImplementation: ABSOperatorImplementation
    {

        public BSRangeCustomOperatorImplementation(  ) : base( ".." )
        {
        }

        public override bool IsCorrectImplementation( ABSObject[] args )
        {
            return args.Length == 2 &&
                   args[0].TryConvertDecimal( out decimal _ ) &&
                   args[1].TryConvertDecimal( out decimal _ );
        }

        protected override ABSObject Execute( ABSObject[] args )
        {
            decimal dStart = args[0].ConvertDecimal();
            decimal dEnd = args[1].ConvertDecimal();

            return new BSRangeEnumerableObject(args[0].Position, (int)dStart, (int)dEnd);
        }

    }

    internal class SimpleCustomExpressionExample
    {


        private static BSEngine CreateEngine()
        {
            Console.WriteLine("Creating Script Engine");
            BSEngineSettings settings = BSEngineSettings.MakeDefault();

            settings.Interfaces.Add(new BSSystemConsoleInterface()); //Add the Console API so we can write things

            return settings.Build();
        }

        private static void Main(string[] args)
        {
            //Create an entry in the Operator Precedence Table.
            BSOperatorPreceedenceTable.Set( new BSBinaryOperator( 3, //The Operator precedence
                                                                  "..", //The Operator that gets matched
                                                                  "op_Range", //The Signature Name
                                                                  2 ) //The Argument Count
                                           );

            BSOperatorImplementationResolver.AddImplementation( null, //Optional Mapping. If set to "op_Range", tables can override this custom operator implementation with their own.
                                                                new BSRangeCustomOperatorImplementation() //Add the Custom implementation.
                                                               );

            BSEngine engine = CreateEngine();

            string source = @"
                foreach i in 0..10 //Using the Custom Range Operator
                {
                    Console.WriteLine(i)
                }
";

            engine.LoadSource( source );

        }
    }
}

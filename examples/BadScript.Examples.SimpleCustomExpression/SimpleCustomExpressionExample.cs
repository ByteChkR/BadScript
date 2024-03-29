﻿using System;
using System.IO;

using BadScript.ConsoleUtils;
using BadScript.Parser.Expressions;
using BadScript.Parser.OperatorImplementations;
using BadScript.Parser.Operators;
using BadScript.Parser.Operators.Implementations;
using BadScript.Serialization;

namespace BadScript.Examples.SimpleCustomExpression
{

    internal class SimpleCustomExpressionExample
    {
        private const string SOURCE = @"
                a = [1, 2, 3, 4]
                b = [5, 6, 7, 8]
                Console.WriteLine(Environment.Debug(a::b))
";


        private static BSEngine CreateEngine()
        {
            Console.WriteLine("Creating Script Engine");
            BSEngineSettings settings = BSEngineSettings.MakeDefault();

            settings.Interfaces.Add(new BSSystemConsoleInterface()); //Add the Console API so we can write things

            return settings.Build();
        }

        private static void ExecuteSerialized(BSEngine engine)
        {
            MemoryStream ms = new MemoryStream();

            BSExpression[] exprs = engine.ParseString(SOURCE);
            BSSerializer.Serialize(exprs, ms);

            ms.Position = 0; //Reset Stream to begin.

            exprs = BSSerializer.Deserialize(ms);
            engine.LoadScript(exprs);
        }

        private static void Main(string[] args)
        {
            //Create an entry in the Operator Precedence Table.
            
            BSOperatorPreceedenceTable.Set(new BSBinaryOperator(3,          //The Operator precedence
                                                                "::",       //The Operator that gets matched
                                                                "op_Join",  //The Signature Name
                                                                2)          //The Argument Count
                                          );

            

            BSOperatorImplementationResolver.AddImplementation("op_Join", //Optional Mapping. If set to "op_Join", tables can override this custom operator implementation with their own.
                                                               new BSJoinCustomOperatorImplementation() //Add the Custom implementation.
                                                              );

            BSEngine engine = CreateEngine();

            //Run from String
            engine.LoadSource(SOURCE);

            //Serialize/Deserialize and Execute
            //Because we use the BSBinaryOperator Class we have serialization for free.
            //For a custom Operator we need to write our own BSExpressionSerializer implementation.
            ExecuteSerialized(engine);
        }
    }

}
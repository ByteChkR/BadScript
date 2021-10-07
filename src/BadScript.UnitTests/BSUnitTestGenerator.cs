using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

using Newtonsoft.Json;

namespace BadScript.UnitTests
{

    public class BSUnitTestInterface: ABSScriptInterface
    {

        private readonly BSEngine m_Engine;
        public BSUnitTestInterface(BSEngine engine) : base( "Testing" )
        {
            m_Engine = engine;
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement( "Run", new BSFunction( "function Run(objCreator, nameOfObj, testDataPath)", RunUnitTests, 3 ) );
        }

        private ABSObject RunUnitTests( ABSObject[] arg )
        {
            BSInterfaceTestCaseData data;
            

            if ( arg[2].IsNull() || !File.Exists(arg[2].ConvertString()) )
                data = new BSInterfaceTestCaseData
                       {
                           FunctionTests = new List < BSInterfaceFunctionTestMatrix >(),
                           PropertyTests = new List < BSInterfacePropertyTest >()
                       };
            else
                data = JsonConvert.DeserializeObject < BSInterfaceTestCaseData >( File.ReadAllText(arg[2].ConvertString()));

            data = data.Merge(arg[0].Invoke(Array.Empty < ABSObject >()).GenerateTestCaseData(arg[1].ConvertString()));

            int failed = 0;

            foreach ( BSRunnableTestCase bsRunnableTestCase in data.GenerateTests() )
            {
                try
                {
                    RunTest( bsRunnableTestCase, arg[1].ConvertString(), arg[0] );
                    ConsoleColor fg = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[PASSED] Test '{bsRunnableTestCase.Key}'");
                    Console.ForegroundColor = fg;
                }
                catch ( Exception e )
                {
                    ConsoleColor fg = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine( $"[FAILED] Test '{bsRunnableTestCase.Key} failed'\n\t{e}" );
                    Console.ForegroundColor = fg;
                    failed++;
                }
            }
            if (!arg[2].IsNull())
                File.WriteAllText( arg[2].ConvertString(), JsonConvert.SerializeObject( data, Formatting.Indented ) );

            return new BSObject( ( decimal )failed );
        }

        private void RunTest( BSRunnableTestCase data, string name, ABSObject obj )
        {
            ABSObject o;

            try
            {
                BSScope scope = new BSScope( m_Engine );
                scope.AddLocalVar( name, obj.Invoke( Array.Empty < ABSObject >() ) );
                o = m_Engine.LoadSource(data.Source, scope, Array.Empty < ABSObject >());

                if (data.CrashIsPass)
                {
                    throw new Exception($"Test Case {data.Key} should crash but did not. Source: {data.Source}");
                }
            }
            catch (Exception e)
            {
                if (!data.CrashIsPass)
                {
                    throw new Exception(
                                        $"Source of test case {data.Key} could not be computed: '{data.Source}'",
                                        e
                                       );
                }

                return;
            }

            if (data.ReturnObjectAction != null)
            {
                try
                {
                    m_Engine.LoadSource(data.ReturnObjectAction, new[] { o });
                }
                catch (Exception e)
                {
                    throw new Exception(
                                        $"Return Object Action of test case {data.Key} could not be computed: '{data.ReturnObjectAction}'",
                                        e
                                       );
                }
            }

        }

    }

    public static class BSUnitTestGenerator
    {

        #region Public

        public static BSInterfaceTestCaseData GenerateTestCaseData( this ABSObject table, string name )
        {
            BSInterfaceTestCaseData data = new BSInterfaceTestCaseData
                                           {
                                               FunctionTests = new List<BSInterfaceFunctionTestMatrix>(),
                                               PropertyTests = new List<BSInterfacePropertyTest>()
                                           };

            data.Populate( table, name );

            return data;
        }

        public static BSInterfaceTestCaseData GenerateTestCaseData( this ABSScriptInterface i )
        {
            

            BSTable root = new BSTable( SourcePosition.Unknown );
            i.AddApi( root );

            return root.GenerateTestCaseData( i.Name );
        }

        #endregion

        #region Private

        private static void Populate( this BSInterfaceTestCaseData data, ABSObject o, string name )
        {
            if ( o is BSFunction f )
            {
                data.Populate( f, name );
            }
            else if ( o is ABSArray a )
            {
                data.Populate( a, name );
            }
            else if ( o is ABSTable t )
            {
                data.Populate( t, name );
            }
            else
            {
                data.PropertyTests.Add( new BSInterfacePropertyTest { Name = name } );
            }
        }

        private static void Populate( this BSInterfaceTestCaseData data, BSFunction f, string name )
        {
            int mmax = f.MaxParameters;

            if ( mmax == int.MaxValue )
            {
                mmax = 3;
            }

            List < BSInterfaceFunctionTest > matrix = new List < BSInterfaceFunctionTest >();

            for ( int i = f.MinParameters; i <= mmax; i++ )
            {
                matrix.Add(
                           new BSInterfaceFunctionTest
                           {
                               Name = "NullArguments",
                               Arguments = Enumerable.Repeat( "null", i ).ToArray()
                           }
                          );
            }

            data.FunctionTests.Add(
                                   new BSInterfaceFunctionTestMatrix
                                   {
                                       Name = name,
                                       Tests = matrix.ToArray()
                                   }
                                  );
        }

        private static void Populate( this BSInterfaceTestCaseData data, ABSArray a, string name )
        {
            for ( int i = 0; i < a.GetLength(); i++ )
            {
                data.Populate( a.GetRawElement( i ), $"{name}[{i}]" );
            }
        }

        private static void Populate( this BSInterfaceTestCaseData data, ABSTable t, string name )
        {
            ABSArray keys = t.Keys;

            for ( int i = 0; i < keys.GetLength(); i++ )
            {
                ABSObject key = keys.GetRawElement( i );
                data.Populate( t.GetRawElement( key ), $"{name}.{key.ConvertString()}" );
            }
        }

        #endregion

    }

}

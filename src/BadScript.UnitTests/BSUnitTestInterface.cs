using System;
using System.Collections.Generic;
using System.IO;

using BadScript.Interfaces;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.UnitTests.DataObjects;

using Newtonsoft.Json;

namespace BadScript.UnitTests
{

    public class BSUnitTestInterface : ABSScriptInterface
    {

        private readonly BSEngine m_Engine;

        #region Public

        public BSUnitTestInterface( BSEngine engine ) : base( "Testing" )
        {
            m_Engine = engine;
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement(
                               "Run",
                               new BSFunction( "function Run(objCreator, nameOfObj, testDataPath)", RunUnitTests, 3 )
                              );
        }

        #endregion

        #region Private

        private void RunTest( BSRunnableTestCase data, string name, ABSObject obj )
        {
            ABSObject o;

            try
            {
                BSScope scope = new BSScope( m_Engine );
                scope.AddLocalVar( name, obj.Invoke( Array.Empty < ABSObject >() ) );
                o = m_Engine.LoadSource( data.Source, scope, Array.Empty < ABSObject >() );

                if ( data.CrashIsPass )
                {
                    throw new Exception( $"Test Case {data.Key} should crash but did not. Source: {data.Source}" );
                }
            }
            catch ( Exception e )
            {
                if ( !data.CrashIsPass )
                {
                    throw new Exception(
                                        $"Source of test case {data.Key} could not be computed: '{data.Source}'",
                                        e
                                       );
                }

                return;
            }

            if ( data.ReturnObjectAction != null )
            {
                try
                {
                    m_Engine.LoadSource( data.ReturnObjectAction, new[] { o } );
                }
                catch ( Exception e )
                {
                    throw new Exception(
                                        $"Return Object Action of test case {data.Key} could not be computed: '{data.ReturnObjectAction}'",
                                        e
                                       );
                }
            }
        }

        private ABSObject RunUnitTests( ABSObject[] arg )
        {
            BSInterfaceTestCaseData data;

            if ( arg[2].IsNull() || !File.Exists( arg[2].ConvertString() ) )
            {
                data = new BSInterfaceTestCaseData
                       {
                           FunctionTests = new List < BSInterfaceFunctionTestMatrix >(),
                           PropertyTests = new List < BSInterfacePropertyTest >()
                       };
            }
            else
            {
                data = JsonConvert.DeserializeObject < BSInterfaceTestCaseData >(
                     File.ReadAllText( arg[2].ConvertString() )
                    );
            }

            data = data.Merge(
                              arg[0].
                                  Invoke( Array.Empty < ABSObject >() ).
                                  GenerateTestCaseData( arg[1].ConvertString() )
                             );

            int failed = 0;

            foreach ( BSRunnableTestCase bsRunnableTestCase in data.GenerateTests() )
            {
                try
                {
                    RunTest( bsRunnableTestCase, arg[1].ConvertString(), arg[0] );
                    ConsoleColor fg = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine( $"[PASSED] Test '{bsRunnableTestCase.Key}'" );
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

            if ( !arg[2].IsNull() )
            {
                File.WriteAllText( arg[2].ConvertString(), JsonConvert.SerializeObject( data, Formatting.Indented ) );
            }

            return new BSObject( ( decimal )failed );
        }

        #endregion

    }

}

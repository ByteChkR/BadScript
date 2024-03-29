﻿using System.Collections.Generic;
using System.Linq;

using BadScript.Interfaces;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.UnitTests.DataObjects;

namespace BadScript.UnitTests
{

    public static class BSUnitTestGenerator
    {

        #region Public

        public static BSInterfaceTestCaseData GenerateTestCaseData( this ABSObject table, string name )
        {
            BSInterfaceTestCaseData data = new BSInterfaceTestCaseData
                                           {
                                               FunctionTests = new List < BSInterfaceFunctionTestMatrix >(),
                                               PropertyTests = new List < BSInterfacePropertyTest >()
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

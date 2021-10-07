using System;

using BadScript.NUnit.Utils;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;

using NUnit.Framework;

namespace BadScript.Tests.FailingTests
{

    public abstract class AFailingBSParserUnitTest : ABSUnitTest
    {

        #region Protected

        protected override void RunTest( string key, Action < ABSObject > returnValidator )
        {
            Assert.True( s_TestFileMap.ContainsKey( key ), $"There is no Test Script for Test Case: {key}" );
            string file = s_TestFileMap[key];

            try
            {
                BSExpression[] exprs = m_Engine.ParseFile( file );
                Assert.Fail( $"The Test case: {key} does not fail as expected." );
            }
            catch ( Exception e )
            {
            }

            returnValidator?.Invoke( BSObject.Null );
        }

        #endregion

    }

}

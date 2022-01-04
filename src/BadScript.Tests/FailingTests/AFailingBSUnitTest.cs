using System;

using BadScript.NUnit.Utils;
using BadScript.Types;
using BadScript.Types.Implementations;

using NUnit.Framework;

namespace BadScript.Tests.FailingTests
{

    public abstract class AFailingBSUnitTest : ABSUnitTest
    {

        #region Protected

        protected override void RunTest( string key, Action < ABSObject > returnValidator )
        {
            Assert.True( s_TestFileMap.ContainsKey( key ), $"There is no Test Script for Test Case: {key}" );
            string file = s_TestFileMap[key];

            try
            {
                m_Engine.LoadFile( file );
                Assert.Fail( $"The Test case: {key} does not fail as expected." );
            }
            catch ( Exception )
            {
            }

            returnValidator?.Invoke( BSObject.Null );
        }

        #endregion

    }

}

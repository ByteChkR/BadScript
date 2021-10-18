using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class ScopeTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/passing/scope/" );
        }

        [Test]
        [TestCaseSource( nameof( GenerateTestCases ) )]
        public void Test( string key )
        {
            RunTest( key, x => Assert.True( x.ConvertBool() ) );
        }

        #endregion

    }

}

using NUnit.Framework;

namespace BadScript.Tests.FailingTests
{

    public class InvalidOperationTests : AFailingBSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/failing/invalid-operations/" );
        }

        [Test]
        [TestCaseSource( nameof( GenerateTestCases ) )]
        public void Test( string key )
        {
            RunTest( key, null );
        }

        #endregion

    }

}

using NUnit.Framework;

namespace BadScript.Tests.FailingTests
{

    public class InvalidSyntaxTests : AFailingBSParserUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/failing/invalid-syntax/" );
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

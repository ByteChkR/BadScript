using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests.Interfaces.Environment
{

    public class EnvironmentHandWrittenTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/interfaces/environment/" );
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

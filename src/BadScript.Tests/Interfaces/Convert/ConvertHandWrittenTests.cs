using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests.Interfaces.Convert
{

    public class ConvertHandWrittenTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/interfaces/convert/" );
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

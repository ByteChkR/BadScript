using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests.Operators
{

    public class MathTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/passing/operators/math/" );
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

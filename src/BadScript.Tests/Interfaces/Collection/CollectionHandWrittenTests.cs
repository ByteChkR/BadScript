using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests.Interfaces.Collection
{

    public class CollectionHandWrittenTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/interfaces/collection/" );
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

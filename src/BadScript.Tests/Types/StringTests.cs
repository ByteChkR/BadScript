using System.Linq;

using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests.Types
{

    public class StringTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap("/tests/passing/types/string/");
        }

        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Test(string key)
        {
            RunTest(key, x => Assert.True(x.ConvertBool()));
        }

        #endregion

    }

}

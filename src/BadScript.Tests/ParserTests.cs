using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class ParserTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap("/tests/passing/parser/");
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

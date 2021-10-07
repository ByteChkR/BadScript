using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests.FailingTests
{

    public class InvalidSyntaxTests : AFailingBSParserUnitTest
    {

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap("/tests/failing/invalid-syntax/");
        }

        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Test(string key)
        {
            RunTest(key, null);
        }

    }

}

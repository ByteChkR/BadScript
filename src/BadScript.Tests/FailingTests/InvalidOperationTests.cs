using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests.FailingTests
{

    public class InvalidOperationTests : AFailingBSUnitTest
    {

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap("/tests/failing/invalid-operations/");
        }

        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Test(string key)
        {
            RunTest(key, null);
        }

    }

}
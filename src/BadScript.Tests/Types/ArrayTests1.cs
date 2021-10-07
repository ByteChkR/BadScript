using BadScript.NUnit.Utils;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class ArrayTests : ABSUnitTest
    {

        public static string[] GenerateTestCases() => PopulateKeyMap( "/tests/types/array/" );
        
        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Test( string key )
        {
            RunTest( key, x => Assert.True( x.ConvertBool() ));
        }

    }

}

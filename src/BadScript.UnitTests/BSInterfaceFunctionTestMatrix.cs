using System.Collections.Generic;

namespace BadScript.NUnit.Utils
{

    public struct BSInterfaceFunctionTestMatrix
    {
        public string ReturnObjectAction;
        public string Name;

        public BSInterfaceFunctionTest[] Tests;

        public IEnumerable < BSRunnableTestCase > GenerateTests()
        {
            foreach ( BSInterfaceFunctionTest bsInterfaceFunctionTest in Tests )
            {
                yield return bsInterfaceFunctionTest.MakeTestCase( Name );
            }
        }
    }

}
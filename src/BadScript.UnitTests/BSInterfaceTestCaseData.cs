using System.Collections.Generic;
using System.Linq;

namespace BadScript.UnitTests
{

    public struct BSInterfaceTestCaseData
    {

        public List < BSInterfacePropertyTest > PropertyTests;
        public List < BSInterfaceFunctionTestMatrix > FunctionTests;

        public IEnumerable < BSRunnableTestCase > GenerateTests()
        {
            foreach ( BSInterfacePropertyTest bsInterfacePropertyTest in PropertyTests )
            {
                yield return bsInterfacePropertyTest.MakeTestCase();
            }

            foreach ( BSInterfaceFunctionTestMatrix bsInterfaceFunctionTestMatrix in FunctionTests )
            {
                foreach ( BSRunnableTestCase bsRunnableTestCase in bsInterfaceFunctionTestMatrix.GenerateTests() )
                {
                    yield return bsRunnableTestCase;
                }
            }
        }

        public BSInterfaceFunctionTestMatrix GetFunctionMatrix( string name )
        {
            return FunctionTests.First( x => x.Name == name );
        }

        public BSInterfacePropertyTest GetPropertyTest( string name )
        {
            return PropertyTests.First( x => x.Name == name );
        }

        public BSInterfaceTestCaseData Merge( BSInterfaceTestCaseData other )
        {
            List < BSInterfacePropertyTest > p = new List < BSInterfacePropertyTest >();
            p.AddRange( PropertyTests );

            foreach ( BSInterfacePropertyTest bsInterfacePropertyTest in other.PropertyTests )
            {
                if ( p.All( x => x.Name != bsInterfacePropertyTest.Name ) )
                {
                    p.Add( bsInterfacePropertyTest );
                }
            }

            List < BSInterfaceFunctionTestMatrix > f = new List < BSInterfaceFunctionTestMatrix >();

            f.AddRange( FunctionTests );

            foreach ( BSInterfaceFunctionTestMatrix bsInterfacePropertyTest in other.FunctionTests )
            {
                if ( f.All( x => x.Name != bsInterfacePropertyTest.Name ) )
                {
                    f.Add( bsInterfacePropertyTest );
                }
            }

            return new BSInterfaceTestCaseData
                   {
                       PropertyTests = p,
                       FunctionTests = f
                   };
        }

    }

}

using System;

using BadScript.NUnit.Utils;
using BadScript.Reflection;

using NUnit.Framework;

namespace BadScript.Tests.Interfaces.Reflection
{

    public class ReflectionTests : ABSUnitTest
    {

        public class TestType
        {

            public static bool StaticProperty { get; set; } = true;
            public string InstanceProperty { get; set; }
            public static bool StaticFunction0()
            {
                return true;
            }
            public static decimal StaticFunction1(decimal input)
            {
                return input * input;
            }
            public bool InstanceFunction0()
            {
                return true;
            }
            public decimal InstanceFunction1(decimal input)
            {
                return input * input;
            }

            public TestType()
            {
                InstanceProperty = "Unset";
            }
            public TestType( string propertyValue )
            {
                InstanceProperty = propertyValue;
            }
        }

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap("/tests/passing/interfaces/reflection/");
        }

        protected override void SetUp( BSEngineSettings settings )
        {
            
            base.SetUp( settings );
            BSReflectionInterface.Instance.AddType<TestType>();

            settings.Interfaces.Add( BSReflectionInterface.Instance );
            settings.ActiveInterfaces.Add( BSReflectionInterface.Instance.Name );
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

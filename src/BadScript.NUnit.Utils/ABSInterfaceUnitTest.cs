﻿using System;
using System.Linq;

using BadScript.Types;

using NUnit.Framework;

namespace BadScript.NUnit.Utils
{

    public abstract class ABSInterfaceUnitTest<T> : ABSUnitTest
        where T: ABSScriptInterfaceUnitTestWrapper, new()
    {

        private static T s_Instance;
        public static T Instance => s_Instance ??= new T();

        private static BSRunnableTestCase[] s_Cases;
        public static string[] GenerateTestCases()
        {
            s_Cases = Instance.TestCases.GenerateTests().ToArray();
            
            return s_Cases.Select(x=>x.Key).ToArray();
        }

        public override void TearDown()
        {
            base.TearDown();
            Instance.TearDown();
        }

        protected override void SetUp( BSEngineSettings settings )
        {
            base.SetUp( settings );
            settings.Interfaces.Add( Instance );
            settings.ActiveInterfaces.Add( Instance.Name );
        }

        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Test(string key)
        {
            RunTest(key, null);
        }

        protected override void RunTest(string key, Action<ABSObject> returnValidator)
        {
            BSRunnableTestCase file = s_Cases.First(x=>x.Key == key);
            ABSObject o;
            try
            {
                o = m_Engine.LoadSource(file.Source);
                if (file.CrashIsPass)
                {
                    Assert.Fail( $"Test Case {file.Key} should crash but did not. Source: {file.Source}" );
                }
            }
            catch ( Exception e )
            {
                if ( file.CrashIsPass )
                {
                    Assert.Pass("Thrown Exception: "+e);
                }
                throw new Exception(
                                    $"Source of test case {file.Key} could not be computed: '{file.Source}'",
                                    e
                                   );
            }

            if ( file.ReturnObjectAction != null )
            {
                try
                {
                    m_Engine.LoadSource(file.ReturnObjectAction, new[] { o });
                }
                catch ( Exception e )
                {
                    throw new Exception(
                                        $"Return Object Action of test case {file.Key} could not be computed: '{file.ReturnObjectAction}'",
                                        e
                                       );
                }
            }
            returnValidator?.Invoke(o);
        }


    }

}
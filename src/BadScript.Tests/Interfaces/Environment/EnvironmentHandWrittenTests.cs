using System;
using System.Collections.Generic;
using System.IO;

using BadScript.ConsoleUtils;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.Interfaces;
using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.Interfaces.Environment;
using BadScript.Interfaces.Versioning;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.NUnit.Utils;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Types;
using BadScript.Xml;
using BadScript.Zip;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class EnvironmentInterfaceTestWrapper:ABSScriptInterfaceUnitTestWrapper
    {

        public EnvironmentInterfaceTestWrapper() : base( new BSEnvironmentInterface(BSEngineSettings.MakeDefault().Build()) )
        {
        }

    }

    public class EnvironmentInterfaceTests : ABSInterfaceUnitTest <EnvironmentInterfaceTestWrapper>
    {



    }

    public class EnvironmentHandWrittenTests : ABSUnitTest
    {
        public static string[] GenerateTestCases() => PopulateKeyMap("/tests/interfaces/environment/");

        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Test(string key)
        {
            RunTest(key, x => Assert.True(x.ConvertBool()));
        }
    }

}

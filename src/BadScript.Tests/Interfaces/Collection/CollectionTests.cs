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

using Newtonsoft.Json;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class CollectionInterfaceTestWrapper : ABSScriptInterfaceUnitTestWrapper
    {
        public CollectionInterfaceTestWrapper() : base( new BSCollectionInterface() )
        {
        }
        
    }
    public class CollectionInterfaceTests : ABSInterfaceUnitTest <CollectionInterfaceTestWrapper>
    {

        

    }
    public class CollectionHandWrittenTests : ABSUnitTest
    {
        public static string[] GenerateTestCases() => PopulateKeyMap("/tests/interfaces/collection/");

        [Test]
        [TestCaseSource(nameof(GenerateTestCases))]
        public void Test(string key)
        {
            RunTest(key, x => Assert.True(x.ConvertBool()));
        }
    }

}
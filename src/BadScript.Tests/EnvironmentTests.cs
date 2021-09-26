using System;
using System.Collections.Generic;
using System.IO;

using BadScript.Common.Types;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.Settings;
using BadScript.StringUtils;
using BadScript.Utils;
using BadScript.Utils.Reflection;
using BadScript.Xml;
using BadScript.Zip;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class EnvironmentTests
    {

        private BSEngine m_Engine;
        private static Dictionary<string, string> m_Files = new Dictionary<string, string>();
        [SetUp]
        public void Setup()
        {
            BSEngineSettings es = BSEngineSettings.MakeDefault();
            es.Interfaces.Add(new BadScriptCoreApi());
            es.Interfaces.Add(new ConsoleApi());
            es.Interfaces.Add(new ConsoleColorApi());
            es.Interfaces.Add(new BS2JsonInterface());
            es.Interfaces.Add(new Json2BSInterface());
            es.Interfaces.Add(new BSFileSystemInterface());
            es.Interfaces.Add(new BSFileSystemPathInterface(AppDomain.CurrentDomain.BaseDirectory));
            es.Interfaces.Add(new BSMathApi());
            es.Interfaces.Add(new HttpApi());
            es.Interfaces.Add(new HttpServerApi());
            es.Interfaces.Add(new ProcessApi());
            es.Interfaces.Add(new StringUtilsApi());
            es.Interfaces.Add(new ZipApi());
            es.Interfaces.Add(new ImagingApi());
            es.Interfaces.Add(new BSReflectionScriptInterface());
            es.Interfaces.Add(new VersionToolsInterface());
            es.Interfaces.Add(new XmlInterface());
            m_Engine = es.Build();
        }
        private static string[] TestFiles()
        {
            string testDir = TestContext.CurrentContext.TestDirectory + "/tests/environment/";
            string[] files = Directory.GetFiles(testDir, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string key = file.Remove(0, testDir.Length);
                m_Files[key] = file;
                files[i] = key;
            }

            return files;

        }

        [Test]
        [TestCaseSource(nameof(TestFiles))]
        public void RunTest(string key)
        {
            string file = m_Files[key];
            ABSObject o = m_Engine.LoadFile(file);
            Assert.IsTrue(o.ConvertBool());
        }
    }

}
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.Interfaces.Versioning;
using BadScript.Settings;
using BadScript.Types;
using BadScript.Types.References;

using NUnit.Framework;

namespace BadScript.NUnit.Utils
{

    public abstract class ABSUnitTest
    {

        internal Dictionary < string, string > TestFileMap => s_TestFileMap;
        protected static readonly Dictionary<string, string> s_TestFileMap = new Dictionary<string, string>();

        protected BSEngine m_Engine;

        #region Public

        [TearDown]
        public virtual void TearDown()
        {

        }
        

        [SetUp]
        public void SetUp()
        {
            BSEngineSettings es = BSEngineSettings.MakeDefault();

            SetUp( es );

            m_Engine = es.Build();
        }

        protected virtual void SetUp( BSEngineSettings settings )
        {
            settings.Interfaces.Add(new BSCollectionInterface());
            settings.Interfaces.Add(new BSConvertInterface());
        }

        protected virtual void RunTest(string key, Action <ABSObject> returnValidator )
        {
            Assert.True( s_TestFileMap.ContainsKey( key ), $"There is no Test Script for Test Case: {key}" );
            string file = s_TestFileMap[key];
            ABSObject o = m_Engine.LoadFile(file);
            returnValidator?.Invoke( o );
        }


        #endregion

        #region Private

        protected static string[] PopulateKeyMap(string testPath)
        {
            string testDir = TestContext.CurrentContext.TestDirectory + testPath;
            string[] files = Directory.GetFiles(testDir, "*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string key = file.Remove(0, testDir.Length);
                s_TestFileMap[key] = file;
                files[i] = key;
            }

            return files;
        }

        #endregion
    }
}

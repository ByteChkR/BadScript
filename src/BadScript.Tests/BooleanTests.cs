using System;
using System.Collections.Generic;
using System.IO;

using BadScript.ConsoleUtils;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.Interfaces.Versioning;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Types;
using BadScript.Xml;
using BadScript.Zip;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class BooleanTests
    {

        private static Dictionary < string, string > m_Files = new Dictionary < string, string >();

        private BSEngine m_Engine;

        #region Public

        [Test]
        [TestCaseSource( nameof( TestFiles ) )]
        public void RunTest( string key )
        {
            string file = m_Files[key];
            ABSObject o = m_Engine.LoadFile( file );
            Assert.IsTrue( o.ConvertBool() );
        }

        [SetUp]
        public void Setup()
        {
            BSEngineSettings es = BSEngineSettings.MakeDefault();
            es.Interfaces.Add(new BSCollectionInterface());
            es.Interfaces.Add(new BSConvertInterface());
            es.Interfaces.Add(new BSSystemConsoleInterface());
            es.Interfaces.Add( new BS2JsonInterface() );
            es.Interfaces.Add( new Json2BSInterface() );
            es.Interfaces.Add( new BSFileSystemInterface() );
            es.Interfaces.Add( new BSFileSystemPathInterface( AppDomain.CurrentDomain.BaseDirectory ) );
            es.Interfaces.Add( new BSMathInterface() );
            es.Interfaces.Add( new BSHttpInterface() );
            es.Interfaces.Add( new BSHttpServerInterface() );
            es.Interfaces.Add( new BSProcessInterface() );
            es.Interfaces.Add( new BSStringInterface() );
            es.Interfaces.Add( new BSZipInterface() );
            es.Interfaces.Add( new BSDrawingInterface() );
            es.Interfaces.Add( new BSVersioningInterface() );
            es.Interfaces.Add( new BSXmlInterface() );
            m_Engine = es.Build();
        }

        #endregion

        #region Private

        private static string[] TestFiles()
        {
            string testDir = TestContext.CurrentContext.TestDirectory + "/tests/literals/boolean/";
            string[] files = Directory.GetFiles( testDir, "*", SearchOption.AllDirectories );

            for ( int i = 0; i < files.Length; i++ )
            {
                string file = files[i];
                string key = file.Remove( 0, testDir.Length );
                m_Files[key] = file;
                files[i] = key;
            }

            return files;
        }

        #endregion

    }

}

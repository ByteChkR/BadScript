using System;
using System.IO;

using BadScript.NUnit.Utils;
using BadScript.Parser.Expressions;
using BadScript.Reflection;
using BadScript.Serialization;
using BadScript.Tests.Interfaces.Reflection;
using BadScript.Types;

using NUnit.Framework;

namespace BadScript.Tests
{

    public class SerializerTests : ABSUnitTest
    {

        #region Public

        public static string[] GenerateTestCases()
        {
            return PopulateKeyMap( "/tests/passing/" );
        }

        [Test]
        [TestCaseSource( nameof( GenerateTestCases ) )]
        public void Test( string key )
        {
            RunTest( key, x => Assert.True( x.ConvertBool() ) );
        }

        #endregion

        #region Protected

        protected override void RunTest( string key, Action < ABSObject > returnValidator )
        {
            Assert.True( s_TestFileMap.ContainsKey( key ), $"There is no Test Script for Test Case: {key}" );
            string file = s_TestFileMap[key];
            BSExpression[] exprs = m_Engine.ParseFile( file );
            MemoryStream ms = new MemoryStream();
            BSSerializer.Serialize( exprs, ms );
            ms.Position = 0;
            exprs = BSSerializer.Deserialize( ms );
            ABSObject o = m_Engine.LoadScript( exprs );
            returnValidator?.Invoke( o );
        }

        protected override void SetUp( BSEngineSettings settings )
        {
            base.SetUp( settings );
            BSReflectionInterface.Instance.AddType < ReflectionTests.TestType >();

            settings.Interfaces.Add( BSReflectionInterface.Instance );
            settings.ActiveInterfaces.Add( BSReflectionInterface.Instance.Name );
        }

        #endregion

    }

}

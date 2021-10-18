using System.IO;

using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.UnitTests;

using NUnit.Framework;

namespace BadScript.Tests.Types
{

    public class StreamTests
    {

        #region Public

        [Test]
        public void RunStreamTests()
        {
            BSEngine e = BSEngineSettings.MakeDefault().Build();
            e.AddInterface( new BSUnitTestInterface( e ) );
            string testCaseDir = Path.Combine( TestContext.CurrentContext.TestDirectory, "badscript-test-data" );
            Directory.CreateDirectory( testCaseDir );
            string testCasePath = Path.Combine( testCaseDir, "StreamObject.json" );

            string source = @"
Environment.LoadInterface(""Testing"")
Environment.Debug(args[0])
return Testing.Run(args[0], ""stream"", args[1]) == 0
";

            e.LoadSource(
                         source,
                         new ABSObject[]
                         {
                             new BSFunction(
                                            "",
                                            a => new BSStreamObject( new MemoryStream( new byte[100] ) ).
                                                GetInnerFunctionTable(),
                                            0
                                           ),
                             new BSObject( testCasePath )
                         }
                        );
        }

        #endregion

    }

}

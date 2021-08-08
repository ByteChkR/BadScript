using System;
using System.Collections.Generic;
using System.Linq;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Interfaces;
using BadScript.Settings;
using BadScript.Tools.CodeGenerator;
using BadScript.Tools.CodeGenerator.Runtime;
using BadScript.Tools.CodeGenerator.Runtime.Attributes;
using BadScript.Utils.Reflection;

namespace BadScript.Testing
{

    public class SubTest
    {
        public string Name;
    }

    public class TestConstructor : WrapperObjectCreator < Test >
    {
    }

    [BSWConstructorCreator( typeof( TestConstructor ) )]
    public class Test
    {
        public float MyValue;
        public bool MyBool;
        public SubTest Sub;

        [BSWIgnore]
        public SubTest IgnoredSub;
        [BSWName( "YEET" )]
        public SubTest RenamedSub;

        public Test()
        {
            Sub = new SubTest();
            IgnoredSub = new SubTest();
            RenamedSub = new SubTest();
        }
    }

    internal class Program
    {
        #region Private

        private static void Main( string[] args )
        {
            string str = WrapperGenerator.Generate < Test >( null, null, "DB" );
            
            //WrapperHelper.AddObjectDB(new DB());
            BSWrapperObject <Test> t = WrapperHelper.Create< Test >(new object[0]);

            ConsoleApi cout = new ConsoleApi();
            BadScriptCoreApi core = new BadScriptCoreApi();

            BSEngine e = new BSEngine(
                BSParserSettings.Default,
                new Dictionary < string, ABSObject >(),
                new List < ABSScriptInterface > { cout, core } );

            e.LoadInterface( "console" );
            e.LoadInterface( "core" );

            e.LoadString( false, "args[0].MyBool = 1\nconsole.print(core.debug(args[0].MyBool))\nconsole.print(core.debug(args[0]))", new ABSObject[] { t } );
        }

        #endregion
    }

}

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

    public static class StaticTest
    {
        public static Test TEST;
    }

    public class SubTest
    {
        public string Name;

        public SubTest Sub => new ();
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

        public static string MyStaticProperty { get; set; } = nameof(MyStaticProperty);
        public static string MyStaticField = nameof(MyStaticField);
        public const string MyConstField = "HELLO";

        public static string MyStaticFunction( string a ) => a + a;

        public static string MyOutFunction( out string a )
        {
            a = "";

            return a;
        }

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
            string str = WrapperGenerator.Generate(typeof(StaticTest), null, null, "DB" , "SDB");
            

            ConsoleApi cout = new ConsoleApi();
            BadScriptCoreApi core = new BadScriptCoreApi();

            DB db = new DB();
            SDB sdb = new SDB();
            ABSObject t = db.Get<Test>(new object[0]);
            BSEngine e = new BSEngine(
                BSParserSettings.Default,
                new Dictionary<string, ABSObject>(),
                new List<ABSScriptInterface> { cout, core, sdb.CreateInterface("test"), db.CreateInterface("test") });

            e.LoadInterface("console");
            e.LoadInterface("core");
            ABSTable t1 = e.LoadInterface("test");

            e.LoadString(false, "args[0].MyBool = 1\nconsole.print(core.debug(args[0].MyBool))\nconsole.print(core.debug(args[0]))\nconsole.print(core.debug(test))\n", new ABSObject[] { t });

        }

        #endregion
    }
    public class BSStaticWrapperObject_BadScript_Testing_StaticTest : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_BadScript_Testing_StaticTest() : base(typeof(BadScript.Testing.StaticTest))
        {
            m_StaticProperties["TEST"] = new BSReflectionReference(() => new BSWrapperObject_BadScript_Testing_Test(BadScript.Testing.StaticTest.TEST), x => BadScript.Testing.StaticTest.TEST = WrapperHelper.UnwrapObject<BadScript.Testing.Test>(x));

        }
    }

    public class BSWrapperObject_BadScript_Testing_Test : BSWrapperObject<BadScript.Testing.Test>

    {
        public BSWrapperObject_BadScript_Testing_Test(BadScript.Testing.Test obj) : base(obj)
        {
            m_Properties["MyValue"] = new BSReflectionReference(() => new BSObject((decimal)m_InternalObject.MyValue), x => m_InternalObject.MyValue = WrapperHelper.UnwrapObject<System.Single>(x));
            m_Properties["MyBool"] = new BSReflectionReference(() => m_InternalObject.MyBool ? BSObject.One : BSObject.Zero, x => m_InternalObject.MyBool = WrapperHelper.UnwrapObject<System.Boolean>(x));
            m_Properties["Sub"] = new BSReflectionReference(() => new BSWrapperObject_BadScript_Testing_SubTest(m_InternalObject.Sub), x => m_InternalObject.Sub = WrapperHelper.UnwrapObject<BadScript.Testing.SubTest>(x));
            m_Properties["YEET"] = new BSReflectionReference(() => new BSWrapperObject_BadScript_Testing_SubTest(m_InternalObject.RenamedSub), x => m_InternalObject.RenamedSub = WrapperHelper.UnwrapObject<BadScript.Testing.SubTest>(x));
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));

        }
    }

    public class BSStaticWrapperObject_BadScript_Testing_Test : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_BadScript_Testing_Test() : base(typeof(BadScript.Testing.Test))
        {
            m_StaticProperties["MyStaticProperty"] = new BSReflectionReference(() => new BSObject(BadScript.Testing.Test.MyStaticProperty), x => BadScript.Testing.Test.MyStaticProperty = WrapperHelper.UnwrapObject<System.String>(x));
            m_StaticProperties["MyStaticField"] = new BSReflectionReference(() => new BSObject(BadScript.Testing.Test.MyStaticField), x => BadScript.Testing.Test.MyStaticField = WrapperHelper.UnwrapObject<System.String>(x));
            m_StaticProperties["MyConstField"] = new BSReflectionReference(() => new BSObject(BadScript.Testing.Test.MyConstField), null);
            m_StaticProperties["MyStaticFunction"] = new BSFunctionReference(new BSFunction("function MyStaticFunction(a)", a => new BSObject(BadScript.Testing.Test.MyStaticFunction(WrapperHelper.UnwrapObject<System.String>(a[0]))), 1));

        }
    }

    public class BSWrapperObject_BadScript_Testing_SubTest : BSWrapperObject<BadScript.Testing.SubTest>

    {
        public BSWrapperObject_BadScript_Testing_SubTest(BadScript.Testing.SubTest obj) : base(obj)
        {
            m_Properties["Sub"] = new BSReflectionReference(() => new BSWrapperObject_BadScript_Testing_SubTest(m_InternalObject.Sub), null);
            m_Properties["Name"] = new BSReflectionReference(() => new BSObject(m_InternalObject.Name), x => m_InternalObject.Name = WrapperHelper.UnwrapObject<System.String>(x));
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));

        }
    }

    public class BSStaticWrapperObject_BadScript_Testing_SubTest : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_BadScript_Testing_SubTest() : base(typeof(BadScript.Testing.SubTest))
        {

        }
    }

    public class BSWrapperObject_System_Object : BSWrapperObject<System.Object>

    {
        public BSWrapperObject_System_Object(System.Object obj) : base(obj)
        {
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));

        }
    }

    public class BSStaticWrapperObject_System_Object : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_System_Object() : base(typeof(System.Object))
        {
            m_StaticProperties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(objA, objB)", a => System.Object.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0]), WrapperHelper.UnwrapObject<System.Object>(a[1])) ? BSObject.One : BSObject.Zero, 2));
            m_StaticProperties["ReferenceEquals"] = new BSFunctionReference(new BSFunction("function ReferenceEquals(objA, objB)", a => System.Object.ReferenceEquals(WrapperHelper.UnwrapObject<System.Object>(a[0]), WrapperHelper.UnwrapObject<System.Object>(a[1])) ? BSObject.One : BSObject.Zero, 2));

        }
    }



    public class DB : IWrapperConstructorDataBase
    {
        private readonly Dictionary<Type, (IWrapperObjectCreator[], Func<object[], object>)> m_Creators;
        public Type[] Types => m_Creators.Keys.ToArray();

        public DB()
        {
            m_Creators = new Dictionary<Type, (IWrapperObjectCreator[], Func<object[], object>)>
            {
{typeof(BadScript.Testing.Test), (new IWrapperObjectCreator[] {new TestConstructor()
}, a => new BSWrapperObject_BadScript_Testing_Test((BadScript.Testing.Test)m_Creators[typeof(BadScript.Testing.Test)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},
{typeof(BadScript.Testing.SubTest), (new IWrapperObjectCreator[] {}, a => new BSWrapperObject_BadScript_Testing_SubTest((BadScript.Testing.SubTest)m_Creators[typeof(BadScript.Testing.SubTest)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},
{typeof(System.Object), (new IWrapperObjectCreator[] {}, a => new BSWrapperObject_System_Object((System.Object)m_Creators[typeof(System.Object)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},

            };
        }
        public bool HasType<T>()
        {
            return m_Creators.ContainsKey(typeof(T));
        }

        public ABSObject Get(Type t, object[] args)
        {
            return (ABSObject)m_Creators[t].Item2(args);
        }
    }



    public class SDB : WrapperStaticDataBase
    {
        public SDB()
        {
            StaticTypes[typeof(BadScript.Testing.StaticTest)] = new BSStaticWrapperObject_BadScript_Testing_StaticTest();
            StaticTypes[typeof(BadScript.Testing.Test)] = new BSStaticWrapperObject_BadScript_Testing_Test();
            StaticTypes[typeof(BadScript.Testing.SubTest)] = new BSStaticWrapperObject_BadScript_Testing_SubTest();
            StaticTypes[typeof(System.Object)] = new BSStaticWrapperObject_System_Object();

        }
    }

}

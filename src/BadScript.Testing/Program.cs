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
            string str = WrapperGenerator.Generate < Test >( null, null, "DB" , "SDB");
            //DB db = new DB();
            //SDB sdb = new SDB();
            //ABSObject t = db.Get<Test>(new object[0]);

            ConsoleApi cout = new ConsoleApi();
            BadScriptCoreApi core = new BadScriptCoreApi();

            //BSEngine e = new BSEngine(
            //    BSParserSettings.Default,
            //    new Dictionary<string, ABSObject>(),
            //    new List<ABSScriptInterface> { cout, core, sdb.CreateInterface("test"), db.CreateInterface("test") });

            //e.LoadInterface("console");
            //e.LoadInterface("core");
            //ABSTable t1 = e.LoadInterface("test");

            //e.LoadString(false, "args[0].MyBool = 1\nconsole.print(core.debug(args[0].MyBool))\nconsole.print(core.debug(args[0]))\nconsole.print(core.debug(test))\n", new ABSObject[] { t });

        }

        #endregion
    }

    public class BSWrapperObject_Test : BSWrapperObject<Test>

    {
        public BSWrapperObject_Test(Test obj) : base(obj)
        {
            m_Properties["MyValue"] = new BSReflectionReference(() => new BSObject((decimal)m_InternalObject.MyValue), x => m_InternalObject.MyValue = WrapperHelper.UnwrapObject<Single>(x));
            m_Properties["MyBool"] = new BSReflectionReference(() => m_InternalObject.MyBool ? BSObject.One : BSObject.Zero, x => m_InternalObject.MyBool = WrapperHelper.UnwrapObject<Boolean>(x));
            m_Properties["Sub"] = new BSReflectionReference(() => new BSWrapperObject_SubTest(m_InternalObject.Sub), x => m_InternalObject.Sub = WrapperHelper.UnwrapObject<SubTest>(x));
            m_Properties["YEET"] = new BSReflectionReference(() => new BSWrapperObject_SubTest(m_InternalObject.RenamedSub), x => m_InternalObject.RenamedSub = WrapperHelper.UnwrapObject<SubTest>(x));
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));

        }
    }

    public class BSStaticWrapperObject_Test : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_Test() : base(typeof(Test))
        {
            m_StaticProperties["MyStaticProperty"] = new BSReflectionReference(() => new BSObject(Test.MyStaticProperty), x => Test.MyStaticProperty = WrapperHelper.UnwrapObject<String>(x));
            m_StaticProperties["MyStaticField"] = new BSReflectionReference(() => new BSObject(Test.MyStaticField), x => Test.MyStaticField = WrapperHelper.UnwrapObject<String>(x));
            m_StaticProperties["MyConstField"] = new BSReflectionReference(() => new BSObject(Test.MyConstField), null);
            m_StaticProperties["MyStaticFunction"] = new BSFunctionReference(new BSFunction("function MyStaticFunction(a)", a => new BSObject(Test.MyStaticFunction(WrapperHelper.UnwrapObject<String>(a[0]))), 1));

        }
    }

    public class BSWrapperObject_SubTest : BSWrapperObject<SubTest>

    {
        public BSWrapperObject_SubTest(SubTest obj) : base(obj)
        {
            m_Properties["Name"] = new BSReflectionReference(() => new BSObject(m_InternalObject.Name), x => m_InternalObject.Name = WrapperHelper.UnwrapObject<String>(x));
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));

        }
    }

    public class BSStaticWrapperObject_SubTest : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_SubTest() : base(typeof(SubTest))
        {

        }
    }

    public class BSWrapperObject_Object : BSWrapperObject<Object>

    {
        public BSWrapperObject_Object(Object obj) : base(obj)
        {

        }
    }

    public class BSStaticWrapperObject_Object : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_Object() : base(typeof(Object))
        {

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
{typeof(Test), (new IWrapperObjectCreator[] {new TestConstructor()
}, a => new BSWrapperObject_Test((Test)m_Creators[typeof(Test)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},
{typeof(SubTest), (new IWrapperObjectCreator[] {}, a => new BSWrapperObject_SubTest((SubTest)m_Creators[typeof(SubTest)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},
{typeof(Object), (new IWrapperObjectCreator[] {}, a => new BSWrapperObject_Object((Object)m_Creators[typeof(Object)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},

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
            StaticTypes[typeof(Test)] = new BSStaticWrapperObject_Test();
            StaticTypes[typeof(SubTest)] = new BSStaticWrapperObject_SubTest();
            StaticTypes[typeof(Object)] = new BSStaticWrapperObject_Object();

        }
    }

}

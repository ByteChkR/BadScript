using System;
using BadScript.Tools.CodeGenerator;
using System.Collections.Generic;
using System.Linq;
using BadScript.Tools.CodeGenerator.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Utils.Reflection;
namespace BadScript.Testing
{
    public class SubGenTest : GenTest
    {
        public string[] strings = { "abc", "def", "ghi", "jkl" };
        public string this[int Index]
        {
            get
            {
                return strings[Index];
            }
            set
            {
                strings[Index] = value;
            }
        }
    }

    public class GenTest
    {
        public static SubGenTest Create() => new SubGenTest();

        public static void Modify( GenTest t )
        {

        }
        public static int Number
        {
            get;
            set;
        }
        public static void Main()
        {
            SDB sdb = new SDB();
            BSWrapperObject_BadScript_Testing_GenTest t =
                new BSWrapperObject_BadScript_Testing_GenTest(new SubGenTest());

            GenTest test = WrapperHelper.UnwrapObject<GenTest>(t);
            ABSObject recast = WrapperHelper.RecastWrapper(t);

            string src = WrapperGenerator.Generate < GenTest >(null, null, "DB", "SDB");
        }
    }




    public class BSWrapperObject_BadScript_Testing_GenTest : BSWrapperObject<BadScript.Testing.GenTest>

    {
        public BSWrapperObject_BadScript_Testing_GenTest(BadScript.Testing.GenTest obj) : base(obj)
        {
            m_Properties["ToString"] = new BSFunctionReference(new BSFunction("function ToString()", a => new BSObject(m_InternalObject.ToString()), 0));
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));
            m_Properties["GetHashCode"] = new BSFunctionReference(new BSFunction("function GetHashCode()", a => new BSObject((decimal)m_InternalObject.GetHashCode()), 0));

        }
    }

    public class BSStaticWrapperObject_BadScript_Testing_GenTest : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_BadScript_Testing_GenTest() : base(typeof(BadScript.Testing.GenTest))
        {
            m_StaticProperties["Number"] = new BSReflectionReference(() => new BSObject((decimal)BadScript.Testing.GenTest.Number), x => BadScript.Testing.GenTest.Number = WrapperHelper.UnwrapObject<System.Int32>(x));
            m_StaticProperties["Create"] = new BSFunctionReference(new BSFunction("function Create()", a => new BSWrapperObject_BadScript_Testing_SubGenTest(BadScript.Testing.GenTest.Create()), 0));
            m_StaticProperties["Modify"] = new BSFunctionReference(new BSFunction("function Modify(t)", a => {
                BadScript.Testing.GenTest.Modify(WrapperHelper.UnwrapObject<BadScript.Testing.GenTest>(a[0]));
                return new BSObject(null);
            }, 1));
            m_StaticProperties["Main"] = new BSFunctionReference(new BSFunction("function Main()", a => {
                BadScript.Testing.GenTest.Main();
                return new BSObject(null);
            }, 0));

        }
        static BSStaticWrapperObject_BadScript_Testing_GenTest()
        {
            WrapperHelper.AddRecastWrapper<BadScript.Testing.GenTest>(o => new BSWrapperObject_BadScript_Testing_GenTest(o));
        }

    }

    public class BSWrapperObject_System_Object : BSWrapperObject<System.Object>

    {
        public BSWrapperObject_System_Object(System.Object obj) : base(obj)
        {
            m_Properties["ToString"] = new BSFunctionReference(new BSFunction("function ToString()", a => new BSObject(m_InternalObject.ToString()), 0));
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));
            m_Properties["GetHashCode"] = new BSFunctionReference(new BSFunction("function GetHashCode()", a => new BSObject((decimal)m_InternalObject.GetHashCode()), 0));

        }
    }

    public class BSStaticWrapperObject_System_Object : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_System_Object() : base(typeof(System.Object))
        {
            m_StaticProperties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(objA, objB)", a => System.Object.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0]), WrapperHelper.UnwrapObject<System.Object>(a[1])) ? BSObject.One : BSObject.Zero, 2));
            m_StaticProperties["ReferenceEquals"] = new BSFunctionReference(new BSFunction("function ReferenceEquals(objA, objB)", a => System.Object.ReferenceEquals(WrapperHelper.UnwrapObject<System.Object>(a[0]), WrapperHelper.UnwrapObject<System.Object>(a[1])) ? BSObject.One : BSObject.Zero, 2));

        }
        static BSStaticWrapperObject_System_Object()
        {
            WrapperHelper.AddRecastWrapper<System.Object>(o => new BSWrapperObject_System_Object(o));
        }

    }

    public class BSWrapperObject_BadScript_Testing_SubGenTest : BSWrapperObject<BadScript.Testing.SubGenTest>

    {
        public BSWrapperObject_BadScript_Testing_SubGenTest(BadScript.Testing.SubGenTest obj) : base(obj)
        {
            m_Properties["get_Item"] = new BSFunctionReference(new BSFunction("function get_Item(Index)", a => new BSObject(m_InternalObject[WrapperHelper.UnwrapObject<System.Int32>(a[0])]), 1));
            m_Properties["set_Item"] = new BSFunctionReference(new BSFunction("function set_Item(Index, value)",
            a => {
                m_InternalObject[WrapperHelper.UnwrapObject<System.Int32>(a[0])] = WrapperHelper.UnwrapObject<System.String>(a[1]);
                return new BSObject(null);
            }
            , 2));
            m_Properties["ToString"] = new BSFunctionReference(new BSFunction("function ToString()", a => new BSObject(m_InternalObject.ToString()), 0));
            m_Properties["Equals"] = new BSFunctionReference(new BSFunction("function Equals(obj)", a => m_InternalObject.Equals(WrapperHelper.UnwrapObject<System.Object>(a[0])) ? BSObject.One : BSObject.Zero, 1));
            m_Properties["GetHashCode"] = new BSFunctionReference(new BSFunction("function GetHashCode()", a => new BSObject((decimal)m_InternalObject.GetHashCode()), 0));

        }
    }

    public class BSStaticWrapperObject_BadScript_Testing_SubGenTest : BSStaticWrapperObject

    {
        public BSStaticWrapperObject_BadScript_Testing_SubGenTest() : base(typeof(BadScript.Testing.SubGenTest))
        {

        }
        static BSStaticWrapperObject_BadScript_Testing_SubGenTest()
        {
            WrapperHelper.AddRecastWrapper<BadScript.Testing.SubGenTest>(o => new BSWrapperObject_BadScript_Testing_SubGenTest(o));
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
{typeof(BadScript.Testing.GenTest), (new IWrapperObjectCreator[] {}, a => new BSWrapperObject_BadScript_Testing_GenTest((BadScript.Testing.GenTest)m_Creators[typeof(BadScript.Testing.GenTest)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},
{typeof(System.Object), (new IWrapperObjectCreator[] {}, a => new BSWrapperObject_System_Object((System.Object)m_Creators[typeof(System.Object)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},
{typeof(BadScript.Testing.SubGenTest), (new IWrapperObjectCreator[] {}, a => new BSWrapperObject_BadScript_Testing_SubGenTest((BadScript.Testing.SubGenTest)m_Creators[typeof(BadScript.Testing.SubGenTest)].Item1.First(x=>x.ArgCount == a.Length).Create(a)))},

            };
        }
        public bool HasType(Type t)
        {
            return m_Creators.ContainsKey(t);
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
            StaticTypes[typeof(BadScript.Testing.GenTest)] = new BSStaticWrapperObject_BadScript_Testing_GenTest();
            StaticTypes[typeof(System.Object)] = new BSStaticWrapperObject_System_Object();
            StaticTypes[typeof(BadScript.Testing.SubGenTest)] = new BSStaticWrapperObject_BadScript_Testing_SubGenTest();

        }
    }


}

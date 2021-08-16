﻿using System;
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
            BSWrapperObject_BadScript_Testing_SubGenTest t =
                new BSWrapperObject_BadScript_Testing_SubGenTest( new SubGenTest() );

            GenTest test = WrapperHelper.UnwrapObject<GenTest>(t);

            string src = WrapperGenerator.Generate < GenTest >();
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
    }

    public class BSWrapperObject_BadScript_Testing_SubGenTest : BSWrapperObject<BadScript.Testing.SubGenTest>

    {
        public BSWrapperObject_BadScript_Testing_SubGenTest(BadScript.Testing.SubGenTest obj) : base(obj)
        {
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
    }

}

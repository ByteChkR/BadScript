using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BadScript.Tools.CodeGenerator.Runtime;
using BadScript.Tools.CodeGenerator.Runtime.Attributes;

namespace BadScript.Tools.CodeGenerator
{
    
public static class WrapperGenerator
{
    private const string DB_STATIC_TEMPLATE = @"
    public class #DBNAME# : WrapperStaticDataBase
    {
        public #DBNAME#()
        {
#CREATORDATA#
        }
    }
";
        private const string DB_TEMPLATE = @"
    public class #DBNAME# : IWrapperConstructorDataBase
    {
        private readonly Dictionary < Type, (IWrapperObjectCreator[], Func < object[], object >) > m_Creators;
        public Type[] Types => m_Creators.Keys.ToArray();

        public #DBNAME#()
        {
            m_Creators = new Dictionary < Type, (IWrapperObjectCreator[], Func < object[], object >) >
            {
#CREATORDATA#
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
    
";

        public static event Action<string> Log = Console.WriteLine;



        public static string GenerateConstructorDataBase(string name, Dictionary<Type, WrapperTypeInfo> wrappers)
        {

            string body = DB_TEMPLATE;

            string GenerateObjectCreator(WrapperTypeInfo info)
            {
                string fmt = "new {0}()";
                string r = "";

                for (int i = 0; i < info.Creators.Length; i++)
                {
                    if (i != 0)
                    {
                        r += ", ";
                    }

                    BSWConstructorCreatorAttribute ca = info.Creators[i];
                    r += string.Format(fmt, ca.ConstructorCreatorType.Name);


                    r += "\n";
                }

                return r;
            }
            string GenerateDataEntry(Type t, WrapperTypeInfo typeInfo)
            {
                return
                    $"{{typeof({t.Name}), (new {nameof(IWrapperObjectCreator)}[] {{{GenerateObjectCreator(typeInfo)}}}, a => new {typeInfo.GeneratedClass}(({t.Name})m_Creators[typeof({t.Name})].Item1.First(x=>x.ArgCount == a.Length).Create(a)))}}";
            }


            string entries = "";
            foreach (KeyValuePair<Type, WrapperTypeInfo> wrapperTypeInfo in wrappers)
            {
                if (wrapperTypeInfo.Value.GeneratedClass.StartsWith("#") ||
                     wrapperTypeInfo.Value.GeneratedClass.StartsWith("@") ||
                     wrapperTypeInfo.Value.GeneratedClass == "BSObject" ||
                     wrapperTypeInfo.Value.GeneratedClass == "")
                {
                    continue;
                }
                entries += GenerateDataEntry(wrapperTypeInfo.Key, wrapperTypeInfo.Value);

                entries += ",";

                entries += "\n";
            }

            return body.Replace("#CREATORDATA#", entries).Replace("#DBNAME#", name);
        }

        public static string GenerateStaticDataBase(string name, Dictionary<Type, WrapperTypeInfo> wrappers)
        {

            string body = DB_STATIC_TEMPLATE;

            string entries = "";
            foreach (KeyValuePair<Type, WrapperTypeInfo> wrapperTypeInfo in wrappers)
            {
                if (wrapperTypeInfo.Value.GeneratedClass.StartsWith("#") ||
                    wrapperTypeInfo.Value.GeneratedClass.StartsWith("@") ||
                    wrapperTypeInfo.Value.GeneratedClass == "BSObject" ||
                    wrapperTypeInfo.Value.GeneratedClass == "")
                {
                    continue;
                }

                entries +=
                    $"StaticTypes[typeof({wrapperTypeInfo.Key.Name})] = new {wrapperTypeInfo.Value.StaticGeneratedClass}()";

                entries += ";";

                entries += "\n";
            }

            return body.Replace("#CREATORDATA#", entries).Replace("#DBNAME#", name);
        }


        private static IWrapperObjectCreator[] GetCreators(Type t)
        {
            IEnumerable<BSWConstructorCreatorAttribute> creatorAttributes =
                t.GetCustomAttributes<BSWConstructorCreatorAttribute>();

            List<IWrapperObjectCreator> creators = new List<IWrapperObjectCreator>();
            foreach (BSWConstructorCreatorAttribute bswConstructorCreatorAttribute in creatorAttributes)
            {
                IWrapperObjectCreator c =
                    (IWrapperObjectCreator)Activator.CreateInstance(
                        bswConstructorCreatorAttribute.ConstructorCreatorType);

                creators.Add(c);
            }

            return creators.ToArray();
        }

        private static string GenerateMethod(MethodInfo mi, Dictionary<Type, WrapperTypeInfo> wrapper, string name = null)
        {
            string sig = "";
            string pName = name ?? mi.Name;
            string dbgSig = "";
            for (int i = 0; i < mi.GetParameters().Length; i++)
            {
                ParameterInfo parameterInfo = mi.GetParameters()[i];

                if (i != 0)
                {
                    dbgSig += ", ";
                    sig += ", ";
                }

                dbgSig += $"{parameterInfo.Name ?? $"_{i}"}";
                sig += $"WrapperHelper.UnwrapObject<{parameterInfo.ParameterType.Name}>(a[{i}])";
            }

            string invocation = $"m_InternalObject.{mi.Name}({sig})";
            string retCreator = wrapper[mi.ReturnType].GetWrapperCode(invocation);
            if (mi.ReturnType == typeof(void))
            {
                retCreator = $"{{\n{invocation};\nreturn new BSObject(null);\n}}";
            }

            string str = $"m_Properties[\"{pName}\"] = new BSFunctionReference(new BSFunction(\"function {pName}({dbgSig})\", a => {retCreator}, {mi.GetParameters().Length}));";

            return str;
        }



        private static string GenerateStaticMethod(MethodInfo mi, Dictionary<Type, WrapperTypeInfo> wrapper, string name = null)
        {
            string sig = "";
            string pName = name ?? mi.Name;
            string dbgSig = "";
            for (int i = 0; i < mi.GetParameters().Length; i++)
            {
                ParameterInfo parameterInfo = mi.GetParameters()[i];

                if (i != 0)
                {
                    dbgSig += ", ";
                    sig += ", ";
                }

                dbgSig += $"{parameterInfo.Name ?? $"_{i}"}";
                sig += $"WrapperHelper.UnwrapObject<{parameterInfo.ParameterType.Name}>(a[{i}])";
            }

            string invocation = $"{mi.DeclaringType.Name}.{mi.Name}({sig})";
            string retCreator = wrapper[mi.ReturnType].GetWrapperCode(invocation);
            if (mi.ReturnType == typeof(void))
            {
                retCreator = $"{{\n{invocation};\nreturn new BSObject(null);\n}}";
            }

            string str = $"m_StaticProperties[\"{pName}\"] = new BSFunctionReference(new BSFunction(\"function {pName}({dbgSig})\", a => {retCreator}, {mi.GetParameters().Length}));";

            return str;
        }

        private static string GenerateField(FieldInfo fi, Dictionary<Type, WrapperTypeInfo> wrapper, string name = null)
        {
            string setter = "null";
            string pName = name ?? fi.Name;

            if ((fi.Attributes & FieldAttributes.InitOnly) == 0)
            {
                setter = $"x=> m_InternalObject.{fi.Name} = WrapperHelper.UnwrapObject<{fi.FieldType.Name}>(x)";
            }
            string str = $"m_Properties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[fi.FieldType].GetWrapperCode($"m_InternalObject.{fi.Name}")}, {setter});";

            return str;
        }

        private static string GenerateStaticField(FieldInfo fi, Dictionary<Type, WrapperTypeInfo> wrapper, string name = null)
        {
            string setter = "null";
            string pName = name ?? fi.Name;

            if ((fi.Attributes & FieldAttributes.InitOnly) == 0)
            {
                setter = $"x=> {fi.DeclaringType.Name}.{fi.Name} = WrapperHelper.UnwrapObject<{fi.FieldType.Name}>(x)";
            }
            string str = $"m_StaticProperties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[fi.FieldType].GetWrapperCode($"{fi.DeclaringType.Name}.{fi.Name}")}, {setter});";

            return str;
        }

        private static string GenerateProperty(PropertyInfo pi, Dictionary<Type, WrapperTypeInfo> wrapper, string name = null)
        {
            string setter = "null";
            string pName = name ?? pi.Name;
            if (pi.CanWrite && pi.SetMethod.IsPublic)
                setter = $"x=> m_InternalObject.{pi.Name} = WrapperHelper.UnwrapObject<{pi.PropertyType.Name}>(x)";
            string str = $"m_Properties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[pi.PropertyType].GetWrapperCode($"m_InternalObject.{pi.Name}")}, {setter});";

            return str;
        }

        private static string GenerateStaticProperty(
            PropertyInfo pi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string setter = "null";
            string pName = name ?? pi.Name;
            if (pi.CanWrite && pi.SetMethod.IsPublic)
                setter = $"x=> {pi.DeclaringType.Name}.{pi.Name} = WrapperHelper.UnwrapObject<{pi.PropertyType.Name}>(x)";
            string str = $"m_StaticProperties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[pi.PropertyType].GetWrapperCode($"{pi.DeclaringType.Name}.{pi.Name}")}, {setter});";

            return str;
        }

        private static string MakeUsings(List<Type> ts)
        {
            string usings = "";
            List<string> ns = new List<string>();

            foreach (Type type in ts)
            {
                if (ns.Contains(type.Namespace))
                    continue;

                ns.Add(type.Namespace);
                usings += $"using {type.Namespace};\n";
            }

            return usings;
        }

        public static string Generate(Type t, string nameSpace = null, Dictionary<Type, WrapperTypeInfo> wrappers = null, string dbName = null, string dbStaticName = null)
        {
            wrappers ??= new Dictionary<Type, WrapperTypeInfo>();
            wrappers[typeof(string)] = new WrapperTypeInfo("", "", "BSObject", "");
            wrappers[typeof(decimal)] = new WrapperTypeInfo("", "","BSObject", "");
            wrappers[typeof(sbyte)] = new WrapperTypeInfo("", "", "#decimal;BSObject", "");
            wrappers[typeof(byte)] = new WrapperTypeInfo("", "", "#decimal;BSObject", "");
            wrappers[typeof(short)] = new WrapperTypeInfo("", "",  "#decimal;BSObject", "");
            wrappers[typeof(ushort)] = new WrapperTypeInfo("", "","#decimal;BSObject", "");
            wrappers[typeof(int)] = new WrapperTypeInfo("", "", "#decimal;BSObject", "");
            wrappers[typeof(uint)] = new WrapperTypeInfo("", "", "#decimal;BSObject", "");
            wrappers[typeof(long)] = new WrapperTypeInfo("", "", "#decimal;BSObject", "");
            wrappers[typeof(ulong)] = new WrapperTypeInfo("", "", "#decimal;BSObject", "");
            wrappers[typeof(float)] = new WrapperTypeInfo("", "",  "#decimal;BSObject", "");
            wrappers[typeof(double)] = new WrapperTypeInfo("", "",  "#decimal;BSObject", "");
            wrappers[typeof(bool)] = new WrapperTypeInfo("", "", "@{0} ? BSObject.One : BSObject.Zero", "");
            wrappers[typeof(void)] = new WrapperTypeInfo("", "", "", "");
            wrappers[typeof(Type)] = new WrapperTypeInfo("", "", "", "");
            (string src, string name) = Generate(t, wrappers);

            string usings = MakeUsings(wrappers.Keys.ToList()) +
                            "\nusing System.Collections.Generic;\nusing System.Linq;\nusing BadScript.Tools.CodeGenerator.Runtime;\r\nusing BadScript.Common.Types;\r\nusing BadScript.Common.Types.Implementations;\r\nusing BadScript.Utils.Reflection;\n\n";

            if (dbName != null)
                src += "\n" + GenerateConstructorDataBase(dbName, wrappers);

            if ( dbStaticName != null )
                src += "\n" + GenerateStaticDataBase( dbStaticName, wrappers );

            if (nameSpace == null)
            {
                return usings + src;
            }
            else
            {
                string ns = $"namespace {nameSpace}";

                return usings + ns + "\n{\n" + src + "\n}\n";
            }




        }


        private static (string, string) Generate(Type t, Dictionary<Type, WrapperTypeInfo> wrappers)
        {
            ObsoleteAttribute obsT = t.GetCustomAttribute<ObsoleteAttribute>();
            BSWIgnoreAttribute ignoreT = t.GetCustomAttribute<BSWIgnoreAttribute>();

            if (ignoreT != null)
            {
                Log(
                    $"Skipping Type: {t.Name}  because it is marked with {nameof(BSWIgnoreAttribute)}");

                return ("", "");
            }
            if (obsT != null && obsT.IsError)
            {
                Log(
                    "Skipping Type: " +
                    t.Name +
                    $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");

                return ("", "");
            }
            if (t.IsArray || t.IsEnum)
            {
                return ("", "");
            }

            if (t.GenericTypeArguments.Length != 0 || t.IsGenericType || t.IsGenericParameter || t.IsGenericTypeDefinition || t.IsConstructedGenericType)
            {
                Log($"Generic Type '{t.Name}' is not Supported.");
                return ("", "");
            }
            else if (t.Name.Contains("+") || t.Name.Contains("&"))
            {
                Log($"Detected unsupported character in type '{t.Name}'.");

                return ("", "");
            }
            else if (wrappers.ContainsKey(t))
            {
                Log($"Already Existing Type: {t.Name}");

                return ("", "");
            }
            else
            {
                Log("Generating Type Wrapper: " + t.Name);
            }


            string className = $"BSWrapperObject_{t.Name}";
            string baseClassName = $"BSWrapperObject<{t.Name}>";
            string sclassName = $"BSStaticWrapperObject_{t.Name}";
            string sbaseClassName = $"BSStaticWrapperObject";
            wrappers.Add(t, new WrapperTypeInfo("", "", "", className));


            StringBuilder sb = new StringBuilder();
            StringBuilder ssb = new StringBuilder();
            PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] spis = t.GetProperties(BindingFlags.Static | BindingFlags.Public);
            FieldInfo[] fis = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] sfis = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            MethodInfo[] mis = t.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            MethodInfo[] smis = t.GetMethods(BindingFlags.Static | BindingFlags.Public);

            List<string> invalidFuncs = new List<string>();
            List<string> invalidStaticFuncs = new List<string>();

            foreach (PropertyInfo propertyInfo in spis)
            {
                ObsoleteAttribute obs = propertyInfo.GetCustomAttribute<ObsoleteAttribute>();
                BSWNameAttribute nameAttrib = propertyInfo.GetCustomAttribute<BSWNameAttribute>();
                BSWIgnoreAttribute ignoreAttrib = propertyInfo.GetCustomAttribute<BSWIgnoreAttribute>();

                if (ignoreAttrib != null)
                {
                    Log(
                        $"Skipping Static Property: {propertyInfo.Name}  because it is marked with {nameof(BSWIgnoreAttribute)}");
                    continue;
                }
                if (obs != null && obs.IsError)
                {
                    Log(
                        "Skipping Static Property: " +
                        propertyInfo.Name +
                        $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");
                    continue;
                }
                Type propType = propertyInfo.PropertyType;

                bool isValid = true;
                if (!wrappers.ContainsKey(propType))
                {
                    (string src, string name) = Generate(propType, wrappers);
                    isValid = src != "";
                }
                else
                {
                    isValid = wrappers[propType].GeneratedClass != "";
                }
                if (!isValid) continue;


                invalidStaticFuncs.Add($"set_{propertyInfo.Name}");
                invalidStaticFuncs.Add($"get_{propertyInfo.Name}");



                ssb.AppendLine(GenerateStaticProperty(propertyInfo, wrappers, nameAttrib?.Name));
            }

            foreach (PropertyInfo propertyInfo in pis)
            {
                ObsoleteAttribute obs = propertyInfo.GetCustomAttribute<ObsoleteAttribute>();
                BSWNameAttribute nameAttrib = propertyInfo.GetCustomAttribute<BSWNameAttribute>();
                BSWIgnoreAttribute ignoreAttrib = propertyInfo.GetCustomAttribute<BSWIgnoreAttribute>();

                if (ignoreAttrib != null)
                {
                    Log(
                        $"Skipping Property: {propertyInfo.Name}  because it is marked with {nameof(BSWIgnoreAttribute)}");
                    continue;
                }
                if (obs != null && obs.IsError)
                {
                    Log(
                        "Skipping Property: " +
                        propertyInfo.Name +
                        $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");
                    continue;
                }
                Type propType = propertyInfo.PropertyType;

                bool isValid = true;
                if (!wrappers.ContainsKey(propType))
                {
                    (string src, string name) = Generate(propType, wrappers);
                    isValid = src != "";
                }
                else
                {
                    isValid = wrappers[propType].GeneratedClass != "";
                }
                if (!isValid) continue;


                invalidFuncs.Add($"set_{propertyInfo.Name}");
                invalidFuncs.Add($"get_{propertyInfo.Name}");



                sb.AppendLine(GenerateProperty(propertyInfo, wrappers, nameAttrib?.Name));
            }

            foreach (FieldInfo fieldInfo in fis)
            {
                BSWNameAttribute nameAttrib = fieldInfo.GetCustomAttribute<BSWNameAttribute>();
                ObsoleteAttribute obs = fieldInfo.GetCustomAttribute<ObsoleteAttribute>();
                BSWIgnoreAttribute ignoreAttrib = fieldInfo.GetCustomAttribute<BSWIgnoreAttribute>();

                if (ignoreAttrib != null)
                {
                    Log(
                        $"Skipping Field: {fieldInfo.Name}  because it is marked with {nameof(BSWIgnoreAttribute)}");
                    continue;
                }
                if (obs != null && obs.IsError)
                {
                    Log(
                        "Skipping Field: " +
                        fieldInfo.Name +
                        $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");
                    continue;
                }
                Type fieldType = fieldInfo.FieldType;

                bool isValid = true;
                if (!wrappers.ContainsKey(fieldType))
                {
                    (string src, string name) = Generate(fieldType, wrappers);
                    isValid = src != "";
                }
                else
                {
                    isValid = wrappers[fieldType].GeneratedClass != "";
                }
                if (!isValid) continue;


                sb.AppendLine(GenerateField(fieldInfo, wrappers, nameAttrib?.Name));
            }

            foreach (FieldInfo fieldInfo in sfis)
            {
                BSWNameAttribute nameAttrib = fieldInfo.GetCustomAttribute<BSWNameAttribute>();
                ObsoleteAttribute obs = fieldInfo.GetCustomAttribute<ObsoleteAttribute>();
                BSWIgnoreAttribute ignoreAttrib = fieldInfo.GetCustomAttribute<BSWIgnoreAttribute>();

                if (ignoreAttrib != null)
                {
                    Log(
                        $"Skipping Static Field: {fieldInfo.Name}  because it is marked with {nameof(BSWIgnoreAttribute)}");
                    continue;
                }
                if (obs != null && obs.IsError)
                {
                    Log(
                        "Skipping Static Field: " +
                        fieldInfo.Name +
                        $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");
                    continue;
                }
                Type fieldType = fieldInfo.FieldType;

                bool isValid = true;
                if (!wrappers.ContainsKey(fieldType))
                {
                    (string src, string name) = Generate(fieldType, wrappers);
                    isValid = src != "";
                }
                else
                {
                    isValid = wrappers[fieldType].GeneratedClass != "";
                }
                if (!isValid) continue;


                ssb.AppendLine(GenerateStaticField(fieldInfo, wrappers, nameAttrib?.Name));
            }

            foreach (MethodInfo methodInfo in mis)
            {
                ObsoleteAttribute obs = methodInfo.GetCustomAttribute<ObsoleteAttribute>();
                BSWNameAttribute nameAttrib = methodInfo.GetCustomAttribute<BSWNameAttribute>();
                BSWIgnoreAttribute ignoreAttrib = methodInfo.GetCustomAttribute<BSWIgnoreAttribute>();

                if (ignoreAttrib != null)
                {
                    Log(
                        $"Skipping Method: {methodInfo.Name}  because it is marked with {nameof(BSWIgnoreAttribute)}");
                    continue;
                }
                if (obs != null && obs.IsError)
                {
                    Log(
                        "Skipping Method: " +
                        methodInfo.Name +
                        $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");
                    continue;
                }
                if (invalidFuncs.Contains(methodInfo.Name))
                    continue;
                Type retType = methodInfo.ReturnType;

                bool isValid = true;
                if (!wrappers.ContainsKey(retType))
                {
                    (string src, string name) = Generate(retType, wrappers);
                    isValid = src != "";
                }
                else
                {
                    isValid = wrappers[retType].GeneratedClass != "";
                }
                if (!isValid) continue;


                ParameterInfo[] paramis = methodInfo.GetParameters();

                foreach (ParameterInfo parameterInfo in paramis)
                {
                    Type pType = parameterInfo.ParameterType;

                    bool isValidFuncParam = true;
                    if (!wrappers.ContainsKey(pType))
                    {
                        (string src, string name) = Generate(pType, wrappers);
                        isValidFuncParam = src != "";
                    }
                    else
                    {
                        isValidFuncParam = wrappers[pType].GeneratedClass != "";
                    }
                    if (!isValidFuncParam) continue;


                    sb.AppendLine(GenerateMethod(methodInfo, wrappers, nameAttrib?.Name));
                }
            }

            foreach (MethodInfo methodInfo in smis)
            {
                ObsoleteAttribute obs = methodInfo.GetCustomAttribute<ObsoleteAttribute>();
                BSWNameAttribute nameAttrib = methodInfo.GetCustomAttribute<BSWNameAttribute>();
                BSWIgnoreAttribute ignoreAttrib = methodInfo.GetCustomAttribute<BSWIgnoreAttribute>();

                if (ignoreAttrib != null)
                {
                    Log(
                        $"Skipping Static Method: {methodInfo.Name}  because it is marked with {nameof(BSWIgnoreAttribute)}");
                    continue;
                }
                if (obs != null && obs.IsError)
                {
                    Log(
                        "Skipping Static Method: " +
                        methodInfo.Name +
                        $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");
                    continue;
                }
                if (invalidFuncs.Contains(methodInfo.Name))
                    continue;
                Type retType = methodInfo.ReturnType;

                bool isValid = true;
                if (!wrappers.ContainsKey(retType))
                {
                    (string src, string name) = Generate(retType, wrappers);
                    isValid = src != "";
                }
                else
                {
                    isValid = wrappers[retType].GeneratedClass != "";
                }
                if (!isValid) continue;


                ParameterInfo[] paramis = methodInfo.GetParameters();

                foreach (ParameterInfo parameterInfo in paramis)
                {
                    Type pType = parameterInfo.ParameterType;

                    bool isValidFuncParam = true;
                    if (!wrappers.ContainsKey(pType))
                    {
                        (string src, string name) = Generate(pType, wrappers);
                        isValidFuncParam = src != "";
                    }
                    else
                    {
                        isValidFuncParam = wrappers[pType].GeneratedClass != "";
                    }
                    if (!isValidFuncParam) continue;


                    ssb.AppendLine(GenerateStaticMethod(methodInfo, wrappers, nameAttrib?.Name));
                }
            }

            string classHeader = $"public class {className} : {baseClassName}\n";
            string classCtor = $"public {className}({t.Name} obj) : base(obj)";
            string ret = classHeader + "\n{\n" + classCtor + "\n{\n" + sb + "\n}\n}\n";

            string sclassHeader = $"public class {sclassName} : {sbaseClassName}\n";
            string sclassCtor = $"public {sclassName}() : base(typeof({t.Name}))";
            string sret = sclassHeader + "\n{\n" + sclassCtor + "\n{\n" + ssb + "\n}\n}\n";

            wrappers[t] = new WrapperTypeInfo(ret, sret, className, sclassName, t.GetCustomAttributes<BSWConstructorCreatorAttribute>().ToArray());



            StringBuilder retB = new StringBuilder();
            foreach (KeyValuePair<Type, WrapperTypeInfo> keyValuePair in wrappers)
            {
                retB.AppendLine(keyValuePair.Value.Source);
                retB.AppendLine( keyValuePair.Value.StaticSource );
            }

            return (retB.ToString(), className);
        }

        public static string Generate<T>(string nameSpace = null, Dictionary<Type, WrapperTypeInfo> wrappers = null, string dbName = null, string staticDBName = null) => Generate(typeof(T), nameSpace, wrappers, dbName, staticDBName);
    }

}

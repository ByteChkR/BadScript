using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BadScript.Tools.CodeGenerator
{
    public static class WrapperGenerator
    {
        public static event Action < string > Log = Console.WriteLine;

        private static string GenerateObjectWrapper(
            string pName,
            Type pType,
            Dictionary < Type, (string, string) > wrapper )
        {
            string name = wrapper[pType].Item2;
            string str = $"new {name}({pName})";

            return str;
        }
        private static string GenerateMethod( MethodInfo mi, Dictionary <Type, (string, string)> wrapper )
        {
            string sig = "";
            string dbgSig = "";
            for ( int i = 0; i < mi.GetParameters().Length; i++ )
            {
                ParameterInfo parameterInfo = mi.GetParameters()[i];

                if ( i != 0 )
                {
                    dbgSig += ", ";
                    sig += ", ";
                }

                dbgSig += $"{parameterInfo.Name ?? $"_{i}"}";
                sig += $"WrapperHelper.UnwrapObject<{parameterInfo.ParameterType.Name}>(a[{i}])";
            }

            string invocation = $"m_InternalObject.{mi.Name}({sig})";
            string retCreator = GenerateObjectWrapper( invocation, mi.ReturnType, wrapper );
             if ( mi.ReturnType == typeof( void ) )
            {
                retCreator = $"{{\n{invocation};\nreturn new BSObject(null);\n}}";
            }

            string str = $"m_Properties.Add(\"{mi.Name}\", new BSFunctionReference(new BSFunction(\"function {mi.Name}({dbgSig})\", a => {retCreator}, {mi.GetParameters().Length})));";

            return str;
        }

        private static string GenerateField(FieldInfo fi, Dictionary<Type, (string, string)> wrapper)
        {
            string setter = "null";

            if ( ( fi.Attributes & FieldAttributes.InitOnly ) == 0 )
            {
                setter = $"x=> m_InternalObject.{fi.Name} = WrapperHelper.UnwrapObject<{fi.FieldType.Name}>(x)";
            }
            string str = $"m_Properties.Add(\"{fi.Name}\", new BSReflectionReference(() => {GenerateObjectWrapper($"m_InternalObject.{fi.Name}", fi.FieldType, wrapper)}, {setter}));";

            return str;
        }

        private static string GenerateProperty(PropertyInfo pi, Dictionary<Type, (string, string)> wrapper)
        {
            string setter = "null";

            if ( pi.CanWrite && pi.SetMethod.IsPublic )
                setter = $"x=> m_InternalObject.{pi.Name} = WrapperHelper.UnwrapObject<{pi.PropertyType.Name}>(x)";
            string str = $"m_Properties.Add(\"{pi.Name}\", new BSReflectionReference(() => {GenerateObjectWrapper($"m_InternalObject.{pi.Name}", pi.PropertyType, wrapper)}, {setter}));";

            return str;
        }

        private static string MakeUsings(List <Type> ts)
        {
            string usings = "";
            List < string > ns = new List < string >();

            foreach ( Type type in ts )
            {
                if ( ns.Contains( type.Namespace ) )
                    continue;

                ns.Add( type.Namespace );
                usings += $"using {type.Namespace};\n";
            }

            return usings;
        }

        public static string Generate( Type t, string nameSpace = null, Dictionary <Type, (string, string)> wrappers=null )
        {
            wrappers ??= new Dictionary < Type, (string, string) >();
            wrappers[typeof(string)] = ("", "");
            wrappers[typeof(decimal)] = ("", "");
            wrappers[typeof(sbyte)] = ("", "");
            wrappers[typeof(byte)] = ("", "");
            wrappers[typeof(short)] = ("", "");
            wrappers[typeof(ushort)] = ("", "");
            wrappers[typeof(int)] = ("", "");
            wrappers[typeof(uint)] = ("", "");
            wrappers[typeof(long)] = ("", "");
            wrappers[typeof(ulong)] = ("", "");
            wrappers[typeof(float)] = ("", "");
            wrappers[typeof(double)] = ("", "");
            wrappers[typeof(bool)] = ("", "");
            wrappers[typeof(void)] = ("", "");
            wrappers[typeof(Type)] = ("", "");
            ( string src, string name ) = Generate( t, wrappers );

            string usings = MakeUsings( wrappers.Keys.ToList() ) +
                            "using BadScript.Tools.CodeGenerator.Runtime;\r\nusing BadScript.Common.Types;\r\nusing BadScript.Common.Types.Implementations;\r\nusing BadScript.Utils.Reflection;\n\n";

            if (nameSpace==null)
            {
                return usings + src;
            }
            else
            {
                string ns = $"namespace {nameSpace}";

                return usings + ns + "\n{\n" + src + "\n}\n";
            }
        }

        private static string GenerateBaseType( Type t, Dictionary < Type, (string, string) > wrappers )
        {
            Type tB = t.BaseType;
            string ret = null;

            while ( tB != null )
            {
                string n = null;
                if(!wrappers.ContainsKey(tB))
                {
                    ( string src, string name ) = Generate( tB, wrappers );
                    n = name;
                }
                else
                {
                    n = wrappers[tB].Item2;
                }
                ret = n;
                tB = tB.BaseType;
            }

            return ret;
        }

        private static (string, string) Generate( Type t , Dictionary <Type, (string, string)> wrappers)
        {
            ObsoleteAttribute obsT = t.GetCustomAttribute<ObsoleteAttribute>();
            if (obsT != null && obsT.IsError)
            {
                Log(
                    "Skipping Type: " +
                    t.Name +
                    $" because it is marked with {nameof(ObsoleteAttribute)} and does not compile(IsError is true)");

                return ( "","");
            }
            if ( t.IsArray )
            {
                return ( "", "" );
            }

            if (t.GenericTypeArguments.Length != 0|| t.IsGenericType || t.IsGenericParameter || t.IsGenericTypeDefinition || t.IsConstructedGenericType )
            {
                Log($"Generic Type '{t.Name}' is not Supported.");
                return ("", "");
            }
            else if ( t.Name.Contains( "+" ) || t.Name.Contains("&"))
            {
                Log($"Detected unsupported character in type '{t.Name}'.");

                return ( "", "" );
            }
            else if ( wrappers.ContainsKey( t ) )
            {
                Log( $"Already Existing Type: {t.Name}" );

                return ( "", "" );
            }
            else
            {
                Log("Generating Type Wrapper: " + t.Name);
            }


            string className = $"BSWrapperObject_{t.Name}";
            string baseClassName = $"BSWrapperObject<{t.Name}>";
            wrappers.Add(t, ("", className));


            StringBuilder sb = new StringBuilder();
            PropertyInfo[] pis = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] fis = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
            MethodInfo[] mis = t.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            List < string > invalidFuncs = new List < string >();
            foreach ( PropertyInfo propertyInfo in pis )
            {
                ObsoleteAttribute obs = propertyInfo.GetCustomAttribute<ObsoleteAttribute>();
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
                if ( !wrappers.ContainsKey( propType ) )
                {
                   (string src, string name) = Generate(propType, wrappers);
                   isValid = src != "";
                }
                else
                {
                    isValid = wrappers[propType].Item1 != "";
                }
                if(!isValid)continue;

                
                invalidFuncs.Add($"set_{propertyInfo.Name}");
                invalidFuncs.Add($"get_{propertyInfo.Name}");
                sb.AppendLine( GenerateProperty( propertyInfo , wrappers) );
            }

            foreach ( FieldInfo fieldInfo in fis )
            {
                ObsoleteAttribute obs = fieldInfo.GetCustomAttribute<ObsoleteAttribute>();
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
                    isValid = wrappers[fieldType].Item1 != "";
                }
                if (!isValid) continue;


                sb.AppendLine( GenerateField( fieldInfo , wrappers) );
            }

            foreach ( MethodInfo methodInfo in mis )
            {
                ObsoleteAttribute obs = methodInfo.GetCustomAttribute < ObsoleteAttribute >();
                if ( obs!=null && obs.IsError )
                {
                    Log(
                        "Skipping Method: " +
                        methodInfo.Name +
                        $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)" );
                    continue;
                }
                if ( invalidFuncs.Contains( methodInfo.Name ) )
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
                    isValid = wrappers[retType].Item1 != "";
                }
                if (!isValid) continue;


                ParameterInfo[] paramis = methodInfo.GetParameters();

                foreach ( ParameterInfo parameterInfo in paramis )
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
                        isValidFuncParam = wrappers[pType].Item1 != "";
                    }
                    if (!isValidFuncParam) continue;


                    sb.AppendLine(GenerateMethod(methodInfo, wrappers));
                }
            }

            string classHeader = $"public class {className} : {baseClassName}\n";
            string classCtor = $"public {className}({t.Name} obj) : base(obj)";

            string ret = classHeader + "\n{\n" + classCtor + "\n{\n" + sb + "\n}\n}\n";

            wrappers[t] = (ret, className);

            StringBuilder retB = new StringBuilder();
            foreach ( KeyValuePair < Type, (string, string) > keyValuePair in wrappers )
            {
                retB.AppendLine( keyValuePair.Value.Item1 );
            }

            return (retB.ToString(), className);
        }

        public static string Generate < T >(string nameSpace = null, Dictionary<Type, (string, string)> wrappers = null) => Generate( typeof( T ), nameSpace, wrappers);
    }

}

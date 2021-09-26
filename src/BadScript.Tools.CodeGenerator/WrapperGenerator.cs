﻿using System;
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
        public bool HasType(Type t)
        {
            return m_Creators.ContainsKey(t);
        }

        public ABSObject Get(Type t, object[] args)
        {
            return (ABSObject)m_Creators[t].Item2(args);
        }
    }
    
";

        public static event Action < string > Log = Console.WriteLine;

        #region Public

        public static string Generate(
            Type t,
            string nameSpace = null,
            Dictionary < Type, WrapperTypeInfo > wrappers = null,
            string dbName = null,
            string dbStaticName = null )
        {
            wrappers ??= new Dictionary < Type, WrapperTypeInfo >();
            wrappers[typeof( string )] = new WrapperTypeInfo( "", "", "BSObject", "" );
            wrappers[typeof( decimal )] = new WrapperTypeInfo( "", "", "BSObject", "" );
            wrappers[typeof( sbyte )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( byte )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( short )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( ushort )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( int )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( uint )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( long )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( ulong )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( float )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( double )] = new WrapperTypeInfo( "", "", "#decimal;BSObject", "" );
            wrappers[typeof( bool )] = new WrapperTypeInfo( "", "", "@{0} ? BSObject.One : BSObject.Zero", "" );
            wrappers[typeof( void )] = new WrapperTypeInfo( "", "", "", "" );
            wrappers[typeof( Type )] = new WrapperTypeInfo( "", "", "", "" );
            ( string _, string _ ) = Generate( t, wrappers );

            string src = GenerateSource( wrappers );

            string usings =
                "\nusing System;\nusing System.Collections.Generic;\nusing System.Linq;\nusing BadScript.Tools.CodeGenerator.Runtime;\r\nusing BadScript.Common.Types;\r\nusing BadScript.Common.Types.Implementations;\r\nusing BadScript.Utils.Reflection;\n\n";

            if ( dbName != null )
            {
                src += "\n" + GenerateConstructorDataBase( dbName, wrappers );
            }

            if ( dbStaticName != null )
            {
                src += "\n" + GenerateStaticDataBase( dbStaticName, wrappers );
            }

            if ( nameSpace == null )
            {
                return usings + src;
            }
            else
            {
                string ns = $"namespace {nameSpace}";

                return usings + ns + "\n{\n" + src + "\n}\n";
            }
        }

        public static string Generate < T >(
            string nameSpace = null,
            Dictionary < Type, WrapperTypeInfo > wrappers = null,
            string dbName = null,
            string staticDBName = null )
        {
            return Generate( typeof( T ), nameSpace, wrappers, dbName, staticDBName );
        }

        public static string GenerateConstructorDataBase( string name, Dictionary < Type, WrapperTypeInfo > wrappers )
        {
            string body = DB_TEMPLATE;

            string GenerateObjectCreator( WrapperTypeInfo info )
            {
                string fmt = "new {0}()";
                string r = "";

                for ( int i = 0; i < info.Creators.Length; i++ )
                {
                    if ( i != 0 )
                    {
                        r += ", ";
                    }

                    BSWConstructorCreatorAttribute ca = info.Creators[i];
                    r += string.Format( fmt, ca.ConstructorCreatorType.Name );

                    r += "\n";
                }

                return r;
            }

            string GenerateDataEntry( Type t, WrapperTypeInfo typeInfo )
            {
                return
                    $"{{typeof({t.FullName}), (new {nameof( IWrapperObjectCreator )}[] {{{GenerateObjectCreator( typeInfo )}}}, a => new {typeInfo.GeneratedClass}(({t.FullName})m_Creators[typeof({t.FullName})].Item1.First(x=>x.ArgCount == a.Length).Create(a)))}}";
            }

            string entries = "";

            foreach ( KeyValuePair < Type, WrapperTypeInfo > wrapperTypeInfo in wrappers )
            {
                if ( wrapperTypeInfo.Value.GeneratedClass.StartsWith( "#" ) ||
                     wrapperTypeInfo.Value.GeneratedClass.StartsWith( "@" ) ||
                     wrapperTypeInfo.Value.GeneratedClass == "BSObject" ||
                     wrapperTypeInfo.Value.GeneratedClass == "" ||
                     wrapperTypeInfo.Key.IsAbstract && wrapperTypeInfo.Key.IsSealed ) //No Static Classes
                {
                    continue;
                }

                entries += GenerateDataEntry( wrapperTypeInfo.Key, wrapperTypeInfo.Value );

                entries += ",";

                entries += "\n";
            }

            return body.Replace( "#CREATORDATA#", entries ).Replace( "#DBNAME#", name );
        }

        public static string GenerateSource( Dictionary < Type, WrapperTypeInfo > wrappers )
        {
            StringBuilder retB = new StringBuilder();

            foreach ( KeyValuePair < Type, WrapperTypeInfo > keyValuePair in wrappers )
            {
                retB.AppendLine( keyValuePair.Value.Source );
                retB.AppendLine( keyValuePair.Value.StaticSource );
            }

            return retB.ToString();
        }

        public static string GenerateStaticDataBase( string name, Dictionary < Type, WrapperTypeInfo > wrappers )
        {
            string body = DB_STATIC_TEMPLATE;

            string entries = "";

            foreach ( KeyValuePair < Type, WrapperTypeInfo > wrapperTypeInfo in wrappers )
            {
                if ( wrapperTypeInfo.Value.GeneratedClass.StartsWith( "#" ) ||
                     wrapperTypeInfo.Value.GeneratedClass.StartsWith( "@" ) ||
                     wrapperTypeInfo.Value.GeneratedClass == "BSObject" ||
                     wrapperTypeInfo.Value.GeneratedClass == "" )
                {
                    continue;
                }

                entries +=
                    $"StaticTypes[typeof({wrapperTypeInfo.Key.FullName})] = new {wrapperTypeInfo.Value.StaticGeneratedClass}()";

                entries += ";";

                entries += "\n";
            }

            return body.Replace( "#CREATORDATA#", entries ).Replace( "#DBNAME#", name );
        }

        #endregion

        #region Private

        private static (string, string) Generate( Type t, Dictionary < Type, WrapperTypeInfo > wrappers )
        {
            ObsoleteAttribute obsT = t.GetCustomAttribute < ObsoleteAttribute >();
            BSWIgnoreAttribute ignoreT = t.GetCustomAttribute < BSWIgnoreAttribute >();

            if ( ignoreT != null )
            {
                Log(
                    $"Skipping Type: {t.FullName}  because it is marked with {nameof( BSWIgnoreAttribute )}"
                   );

                return ( "", "" );
            }

            if ( obsT != null && obsT.IsError )
            {
                Log(
                    "Skipping Type: " +
                    t.FullName +
                    $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)"
                   );

                return ( "", "" );
            }

            if ( t.IsArray || t.IsEnum )
            {
                Log( $"Arrays and Enums are currently not Supported" );

                return ( "", "" );
            }

            if ( t.GenericTypeArguments.Length != 0 ||
                 t.IsGenericType ||
                 t.IsGenericParameter ||
                 t.IsGenericTypeDefinition ||
                 t.IsConstructedGenericType )
            {
                Log( $"Generic Type '{t.FullName}' is not Supported." );

                return ( "", "" );
            }
            else if ( t.FullName.Contains( "+" ) ||
                      t.FullName.Contains( "&" ) ||
                      t.FullName.Contains( "*" ) ||
                      t.FullName.Contains( "<" ) ||
                      t.FullName.Contains( ">" ) ||
                      t.FullName.Contains( "`" ) )
            {
                Log( $"Detected unsupported character in type '{t.FullName}'." );

                return ( "", "" );
            }
            else if ( wrappers.ContainsKey( t ) )
            {
                Log( $"Already Existing Type: {t.FullName}" );

                return ( "", "" );
            }
            else
            {
                Log( "Generating Type Wrapper: " + t.FullName );
            }

            string MakeValidType( string n )
            {
                return n.Replace( ".", "_" );
            }

            string className = $"BSWrapperObject_{MakeValidType( t.FullName )}";
            string baseClassName = $"BSWrapperObject<{t.FullName}>";
            string sclassName = $"BSStaticWrapperObject_{MakeValidType( t.FullName )}";
            string sbaseClassName = $"BSStaticWrapperObject";
            wrappers.Add( t, new WrapperTypeInfo( "", "", className, sclassName ) );

            StringBuilder sb = new StringBuilder();
            StringBuilder ssb = new StringBuilder();
            PropertyInfo[] pis = t.GetProperties( BindingFlags.Instance | BindingFlags.Public );
            PropertyInfo[] spis = t.GetProperties( BindingFlags.Static | BindingFlags.Public );
            FieldInfo[] fis = t.GetFields( BindingFlags.Instance | BindingFlags.Public );
            FieldInfo[] sfis = t.GetFields( BindingFlags.Static | BindingFlags.Public );
            MethodInfo[] mis = t.GetMethods( BindingFlags.Instance | BindingFlags.Public );
            MethodInfo[] smis = t.GetMethods( BindingFlags.Static | BindingFlags.Public );

            List < string > invalidFuncs = new List < string >();
            List < string > invalidStaticFuncs = new List < string >();

            foreach ( PropertyInfo propertyInfo in spis )
            {
                ObsoleteAttribute obs = propertyInfo.GetCustomAttribute < ObsoleteAttribute >();
                BSWNameAttribute nameAttrib = propertyInfo.GetCustomAttribute < BSWNameAttribute >();
                BSWIgnoreAttribute ignoreAttrib = propertyInfo.GetCustomAttribute < BSWIgnoreAttribute >();

                invalidStaticFuncs.Add( $"set_{propertyInfo.Name}" );
                invalidStaticFuncs.Add( $"get_{propertyInfo.Name}" );

                if ( ignoreAttrib != null )
                {
                    Log(
                        $"Skipping Static Property: {propertyInfo.Name}  because it is marked with {nameof( BSWIgnoreAttribute )}"
                       );

                    continue;
                }

                if ( obs != null && obs.IsError )
                {
                    Log(
                        "Skipping Static Property: " +
                        propertyInfo.Name +
                        $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)"
                       );

                    continue;
                }

                Type propType = propertyInfo.PropertyType;

                if ( string.IsNullOrEmpty( propType.FullName ) )
                {
                    Log( $"Invalid Empty Type: {propType} in property {propertyInfo}" );

                    continue;
                }

                if ( !IsValidType( propType, wrappers ) )
                {
                    Log( $"Property Type '{propType}' is not a Valid Type in property {propertyInfo}" );

                    continue;
                }

                ssb.AppendLine( GenerateStaticProperty( propertyInfo, wrappers, nameAttrib?.Name ) );
            }

            foreach ( PropertyInfo propertyInfo in pis )
            {
                ObsoleteAttribute obs = propertyInfo.GetCustomAttribute < ObsoleteAttribute >();
                BSWNameAttribute nameAttrib = propertyInfo.GetCustomAttribute < BSWNameAttribute >();
                BSWIgnoreAttribute ignoreAttrib = propertyInfo.GetCustomAttribute < BSWIgnoreAttribute >();

                invalidFuncs.Add( $"set_{propertyInfo.Name}" );
                invalidFuncs.Add( $"get_{propertyInfo.Name}" );

                if ( ignoreAttrib != null )
                {
                    Log(
                        $"Skipping Property: {propertyInfo.Name}  because it is marked with {nameof( BSWIgnoreAttribute )}"
                       );

                    continue;
                }

                if ( obs != null && obs.IsError )
                {
                    Log(
                        "Skipping Property: " +
                        propertyInfo.Name +
                        $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)"
                       );

                    continue;
                }

                if ( propertyInfo.Name == "Item" && propertyInfo.GetIndexParameters().Length != 0 )
                {
                    Log( $"Found Index Accessor in Type: {t}." );

                    if ( !IsValidType( propertyInfo.PropertyType, wrappers ) )
                    {
                        Log(
                            $"Property Type '{propertyInfo.PropertyType}' is not a Valid Type in property {propertyInfo}"
                           );

                        continue;
                    }

                    IEnumerable < Type > ptypes = propertyInfo.GetIndexParameters().Select( x => x.ParameterType );

                    bool valid = true;

                    foreach ( Type ptype in ptypes )
                    {
                        if ( ptype.GenericTypeArguments.Length != 0 ||
                             ptype.IsGenericType ||
                             ptype.IsGenericParameter ||
                             ptype.IsGenericTypeDefinition ||
                             ptype.IsConstructedGenericType )
                        {
                            valid = false;
                            Log( $"Generic Type '{ptype}' Not Supported in Property '{propertyInfo}'" );

                            continue;
                        }
                    }

                    if ( !valid )
                    {
                        continue;
                    }

                    string s = GenerateArrayAccessor( propertyInfo, wrappers, nameAttrib?.Name );
                    sb.AppendLine( s );

                    continue;
                }

                Type propType = propertyInfo.PropertyType;

                if ( string.IsNullOrEmpty( propType.FullName ) )
                {
                    Log( $"Invalid Empty Type: {propType} in property {propertyInfo}" );

                    continue;
                }

                if ( !IsValidType( propType, wrappers ) )
                {
                    Log( $"Property Type '{propType}' is not a Valid Type in property {propertyInfo}" );

                    continue;
                }

                sb.AppendLine( GenerateProperty( propertyInfo, wrappers, nameAttrib?.Name ) );
            }

            foreach ( FieldInfo fieldInfo in fis )
            {
                BSWNameAttribute nameAttrib = fieldInfo.GetCustomAttribute < BSWNameAttribute >();
                ObsoleteAttribute obs = fieldInfo.GetCustomAttribute < ObsoleteAttribute >();
                BSWIgnoreAttribute ignoreAttrib = fieldInfo.GetCustomAttribute < BSWIgnoreAttribute >();

                if ( ignoreAttrib != null )
                {
                    Log(
                        $"Skipping Field: {fieldInfo.Name}  because it is marked with {nameof( BSWIgnoreAttribute )}"
                       );

                    continue;
                }

                if ( obs != null && obs.IsError )
                {
                    Log(
                        "Skipping Field: " +
                        fieldInfo.Name +
                        $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)"
                       );

                    continue;
                }

                Type fieldType = fieldInfo.FieldType;

                if ( string.IsNullOrEmpty( fieldType.FullName ) )
                {
                    Log( $"Invalid Empty Type: {fieldType} in field {fieldInfo}" );

                    continue;
                }

                if ( !IsValidType( fieldType, wrappers ) )
                {
                    Log( $"Return Type '{fieldType}' is not a Valid Type in field {fieldInfo}" );

                    continue;
                }

                sb.AppendLine( GenerateField( fieldInfo, wrappers, nameAttrib?.Name ) );
            }

            foreach ( FieldInfo fieldInfo in sfis )
            {
                BSWNameAttribute nameAttrib = fieldInfo.GetCustomAttribute < BSWNameAttribute >();
                ObsoleteAttribute obs = fieldInfo.GetCustomAttribute < ObsoleteAttribute >();
                BSWIgnoreAttribute ignoreAttrib = fieldInfo.GetCustomAttribute < BSWIgnoreAttribute >();

                if ( ignoreAttrib != null )
                {
                    Log(
                        $"Skipping Static Field: {fieldInfo.Name}  because it is marked with {nameof( BSWIgnoreAttribute )}"
                       );

                    continue;
                }

                if ( obs != null && obs.IsError )
                {
                    Log(
                        "Skipping Static Field: " +
                        fieldInfo.Name +
                        $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)"
                       );

                    continue;
                }

                Type fieldType = fieldInfo.FieldType;

                if ( string.IsNullOrEmpty( fieldType.FullName ) )
                {
                    Log( $"Invalid Empty Type: {fieldType} in field {fieldInfo}" );

                    continue;
                }

                if ( !IsValidType( fieldType, wrappers ) )
                {
                    Log( $"Return Type '{fieldType}' is not a Valid Type in field {fieldInfo}" );

                    continue;
                }

                ssb.AppendLine( GenerateStaticField( fieldInfo, wrappers, nameAttrib?.Name ) );
            }

            foreach ( MethodInfo methodInfo in mis )
            {
                ObsoleteAttribute obs = methodInfo.GetCustomAttribute < ObsoleteAttribute >();
                BSWNameAttribute nameAttrib = methodInfo.GetCustomAttribute < BSWNameAttribute >();
                BSWIgnoreAttribute ignoreAttrib = methodInfo.GetCustomAttribute < BSWIgnoreAttribute >();

                if ( ignoreAttrib != null )
                {
                    Log(
                        $"Skipping Method: {methodInfo.Name}  because it is marked with {nameof( BSWIgnoreAttribute )}"
                       );

                    continue;
                }

                if ( obs != null && obs.IsError )
                {
                    Log(
                        "Skipping Method: " +
                        methodInfo.Name +
                        $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)"
                       );

                    continue;
                }

                if ( methodInfo.GetParameters().
                                Any( x => x.IsOut || string.IsNullOrEmpty( x.ParameterType.FullName ) ) )
                {
                    Log( $"Skipping Method '{methodInfo.Name}' because it has an out Parameter" );

                    continue;
                }

                IEnumerable < Type > ptypes = methodInfo.GetParameters().Select( x => x.ParameterType );

                foreach ( Type ptype in ptypes )
                {
                    if ( ptype.GenericTypeArguments.Length != 0 ||
                         ptype.IsGenericType ||
                         ptype.IsGenericParameter ||
                         ptype.IsGenericTypeDefinition ||
                         ptype.IsConstructedGenericType )
                    {
                        Log( $"Generic Type '{ptype}' Not Supported in function '{methodInfo}'" );

                        continue;
                    }
                }

                if ( invalidFuncs.Contains( methodInfo.Name ) )
                {
                    Log( $"Skipping Invalid Function: {methodInfo.Name}" );

                    continue;
                }

                Type retType = methodInfo.ReturnType;

                if ( string.IsNullOrEmpty( retType.FullName ) )
                {
                    Log( $"Invalid Empty Type: {retType} in method {methodInfo}" );

                    continue;
                }

                if ( !IsValidType( retType, wrappers ) )
                {
                    Log( $"Return Type '{retType}' is not a Valid Type in method '{methodInfo}'" );

                    continue;
                }

                bool cont = false;

                foreach ( ParameterInfo miPi in methodInfo.GetParameters() )
                {
                    if ( !IsValidType( miPi.ParameterType, wrappers ) )
                    {
                        Log( $"Parameter Type '{retType}' is not a Valid Type in function '{methodInfo}'" );
                        cont = true;
                    }
                }

                if ( cont )
                {
                    continue;
                }

                sb.AppendLine( GenerateMethod( methodInfo, wrappers, nameAttrib?.Name ) );
            }

            foreach ( MethodInfo methodInfo in smis )
            {
                ObsoleteAttribute obs = methodInfo.GetCustomAttribute < ObsoleteAttribute >();
                BSWNameAttribute nameAttrib = methodInfo.GetCustomAttribute < BSWNameAttribute >();
                BSWIgnoreAttribute ignoreAttrib = methodInfo.GetCustomAttribute < BSWIgnoreAttribute >();

                if ( ignoreAttrib != null )
                {
                    Log(
                        $"Skipping Static Method: {methodInfo.Name}  because it is marked with {nameof( BSWIgnoreAttribute )}"
                       );

                    continue;
                }

                if ( obs != null && obs.IsError )
                {
                    Log(
                        "Skipping Static Method: " +
                        methodInfo.Name +
                        $" because it is marked with {nameof( ObsoleteAttribute )} and does not compile(IsError is true)"
                       );

                    continue;
                }

                if ( methodInfo.GetParameters().
                                Any( x => x.IsOut || string.IsNullOrEmpty( x.ParameterType.FullName ) ) )
                {
                    Log( $"Skipping Static Method '{methodInfo.Name}' because it has an out Parameter" );

                    continue;
                }

                if ( methodInfo.Name.StartsWith( "op_" ) )
                {
                    Log(
                        "Skipping Static Operator Overrides: " +
                        methodInfo.Name +
                        $" as it is currently not supported"
                       );

                    continue;
                }

                IEnumerable < Type > ptypes = methodInfo.GetParameters().Select( x => x.ParameterType );

                foreach ( Type ptype in ptypes )
                {
                    if ( ptype.GenericTypeArguments.Length != 0 ||
                         ptype.IsGenericType ||
                         ptype.IsGenericParameter ||
                         ptype.IsGenericTypeDefinition ||
                         ptype.IsConstructedGenericType )
                    {
                        Log( $"Generic Type '{ptype}' Not Supported in function '{methodInfo}'" );

                        continue;
                    }
                }

                if ( invalidStaticFuncs.Contains( methodInfo.Name ) )
                {
                    Log( $"Skipping Invalid Function: {methodInfo.Name}" );

                    continue;
                }

                Type retType = methodInfo.ReturnType;

                if ( string.IsNullOrEmpty( retType.FullName ) )
                {
                    Log( $"Invalid Empty Type: {retType} in method {methodInfo}" );

                    continue;
                }

                if ( !IsValidType( retType, wrappers ) )
                {
                    Log( $"Return Type '{retType}' is not a Valid Type in function '{methodInfo}'" );

                    continue;
                }

                bool cont = false;

                foreach ( ParameterInfo miPi in methodInfo.GetParameters() )
                {
                    if ( !IsValidType( miPi.ParameterType, wrappers ) )
                    {
                        Log( $"Parameter Type '{retType}' is not a Valid Type in function '{methodInfo}'" );
                        cont = true;
                    }
                }

                if ( cont )
                {
                    continue;
                }

                ssb.AppendLine( GenerateStaticMethod( methodInfo, wrappers, nameAttrib?.Name ) );
            }

            string classHeader = $"public class {className} : {baseClassName}\n";
            string classCtor = $"public {className}({t.FullName} obj) : base(obj)";
            string ret = classHeader + "\n{\n" + classCtor + "\n{\n" + sb + "\n}\n}\n";

            if ( t.IsAbstract && t.IsSealed )
            {
                ret = "";
            }

            string classStaticCtor =
                $"static {sclassName}()\n{{\nWrapperHelper.AddRecastWrapper<{t.FullName}>(o => new {className}(o));\n}}\n";

            string sclassHeader = $"public class {sclassName} : {sbaseClassName}\n";
            string sclassCtor = $"public {sclassName}() : base(typeof({t.FullName}))";
            string sret = sclassHeader + "\n{\n" + sclassCtor + "\n{\n" + ssb + "\n}\n" + classStaticCtor + "\n}\n";

            wrappers[t] = new WrapperTypeInfo(
                                              ret,
                                              sret,
                                              className,
                                              sclassName,
                                              t.GetCustomAttributes < BSWConstructorCreatorAttribute >().ToArray()
                                             );

            Type baseType = t.BaseType;

            while ( baseType != null )
            {
                Generate( baseType, wrappers );
                baseType = baseType.BaseType;
            }

            StringBuilder retB = new StringBuilder();

            foreach ( KeyValuePair < Type, WrapperTypeInfo > keyValuePair in wrappers )
            {
                retB.AppendLine( keyValuePair.Value.Source );
                retB.AppendLine( keyValuePair.Value.StaticSource );
            }

            return ( ret, className );
        }

        private static string GenerateArrayAccessor(
            PropertyInfo mi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string pName = name ?? mi.Name;
            string str = GenerateArrayAccessorGetMethod( mi.GetMethod, wrapper, "get_" + pName );

            if ( mi.SetMethod == null )
            {
                return str;
            }

            return str +
                   "\n" +
                   GenerateArrayAccessorSetMethod( mi.SetMethod, mi.PropertyType, wrapper, "set_" + pName );
        }

        private static string GenerateArrayAccessorGetMethod(
            MethodInfo mi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
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
                sig += $"WrapperHelper.UnwrapObject<{parameterInfo.ParameterType.FullName}>(a[{i}])";
            }

            string invocation = $"m_InternalObject[{sig}]";
            string retCreator = wrapper[mi.ReturnType].GetWrapperCode( invocation );

            if ( mi.ReturnType == typeof( void ) )
            {
                retCreator = $"{{\n{invocation};\nreturn new BSObject(null);\n}}";
            }

            Log( $"Generating Array Access Method: {mi.DeclaringType.FullName}.{name}({sig})" );

            string str =
                $"m_Properties[\"{name}\"] = new BSFunctionReference(new BSFunction(\"function {name}({dbgSig})\", a => {retCreator}, {mi.GetParameters().Length}));";

            return str;
        }

        private static string GenerateArrayAccessorSetMethod(
            MethodInfo mi,
            Type pType,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string sig = "";
            string dbgSig = "";

            for ( int i = 0; i < mi.GetParameters().Length; i++ )
            {
                ParameterInfo parameterInfo = mi.GetParameters()[i];

                if ( i != 0 )
                {
                    dbgSig += ", ";

                    if ( i != mi.GetParameters().Length - 1 )
                    {
                        sig += ", ";
                    }
                }

                dbgSig += $"{parameterInfo.Name ?? $"_{i}"}";

                if ( i != mi.GetParameters().Length - 1 )
                {
                    sig += $"WrapperHelper.UnwrapObject<{parameterInfo.ParameterType.FullName}>(a[{i}])";
                }
            }

            string setter =
                $"\na=> {{\n m_InternalObject[{sig}] = WrapperHelper.UnwrapObject<{pType.FullName}>(a[{mi.GetParameters().Length - 1}]);\nreturn new BSObject(null);\n}}\n";

            Log( $"Generating Array Access Method: {mi.DeclaringType.FullName}.{name}({sig})" );

            string str =
                $"m_Properties[\"{name}\"] = new BSFunctionReference(new BSFunction(\"function {name}({dbgSig})\", {setter}, {mi.GetParameters().Length}));";

            return str;
        }

        private static string GenerateField(
            FieldInfo fi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string setter = "null";
            string pName = name ?? fi.Name;

            if ( !fi.IsLiteral && !fi.IsInitOnly )
            {
                setter = $"x=> m_InternalObject.{fi.Name} = WrapperHelper.UnwrapObject<{fi.FieldType.FullName}>(x)";
            }

            Log( $"Generating Field: {fi.DeclaringType.FullName}.{fi.Name}({pName})" );

            string str =
                $"m_Properties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[fi.FieldType].GetWrapperCode( $"m_InternalObject.{fi.Name}" )}, {setter});";

            return str;
        }

        private static string GenerateMethod(
            MethodInfo mi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string sig = "";
            string pName = name ?? mi.Name;
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
                sig += $"WrapperHelper.UnwrapObject<{parameterInfo.ParameterType.FullName}>(a[{i}])";
            }

            string invocation = $"m_InternalObject.{mi.Name}({sig})";
            string retCreator = wrapper[mi.ReturnType].GetWrapperCode( invocation );

            if ( mi.ReturnType == typeof( void ) )
            {
                retCreator = $"{{\n{invocation};\nreturn new BSObject(null);\n}}";
            }

            Log( $"Generating Method: {mi.DeclaringType.FullName}.{mi.Name}({pName})" );

            string str =
                $"m_Properties[\"{pName}\"] = new BSFunctionReference(new BSFunction(\"function {pName}({dbgSig})\", a => {retCreator}, {mi.GetParameters().Length}));";

            return str;
        }

        private static string GenerateProperty(
            PropertyInfo pi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string setter = "null";
            string pName = name ?? pi.Name;

            if ( pi.CanWrite && pi.SetMethod.IsPublic )
            {
                setter = $"x=> m_InternalObject.{pi.Name} = WrapperHelper.UnwrapObject<{pi.PropertyType.FullName}>(x)";
            }

            Log( $"Generating Property: {pi.DeclaringType.FullName}.{pi.Name}({pName})" );

            string str =
                $"m_Properties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[pi.PropertyType].GetWrapperCode( $"m_InternalObject.{pi.Name}" )}, {setter});";

            return str;
        }

        private static string GenerateStaticField(
            FieldInfo fi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string setter = "null";
            string pName = name ?? fi.Name;

            if ( !fi.IsLiteral && !fi.IsInitOnly )
            {
                setter =
                    $"x=> {fi.DeclaringType.FullName}.{fi.Name} = WrapperHelper.UnwrapObject<{fi.FieldType.FullName}>(x)";
            }

            Log( $"Generating Static Field: {fi.DeclaringType.FullName}.{fi.Name}({pName})" );

            string str =
                $"m_StaticProperties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[fi.FieldType].GetWrapperCode( $"{fi.DeclaringType.FullName}.{fi.Name}" )}, {setter});";

            return str;
        }

        private static string GenerateStaticMethod(
            MethodInfo mi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string sig = "";
            string pName = name ?? mi.Name;
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
                sig += $"WrapperHelper.UnwrapObject<{parameterInfo.ParameterType.FullName}>(a[{i}])";
            }

            string invocation = $"{mi.DeclaringType.FullName}.{mi.Name}({sig})";
            string retCreator = wrapper[mi.ReturnType].GetWrapperCode( invocation );

            if ( mi.ReturnType == typeof( void ) )
            {
                retCreator = $"{{\n{invocation};\nreturn new BSObject(null);\n}}";
            }

            Log( $"Generating Static Method: {mi.DeclaringType.FullName}.{mi.Name}({pName})" );

            string str =
                $"m_StaticProperties[\"{pName}\"] = new BSFunctionReference(new BSFunction(\"function {pName}({dbgSig})\", a => {retCreator}, {mi.GetParameters().Length}));";

            return str;
        }

        private static string GenerateStaticProperty(
            PropertyInfo pi,
            Dictionary < Type, WrapperTypeInfo > wrapper,
            string name = null )
        {
            string setter = "null";
            string pName = name ?? pi.Name;

            if ( pi.CanWrite && pi.SetMethod.IsPublic )
            {
                setter =
                    $"x=> {pi.DeclaringType.FullName}.{pi.Name} = WrapperHelper.UnwrapObject<{pi.PropertyType.FullName}>(x)";
            }

            Log( $"Generating Static Property: {pi.DeclaringType.FullName}.{pi.Name}({pName})" );

            string str =
                $"m_StaticProperties[\"{pName}\"] = new BSReflectionReference(() => {wrapper[pi.PropertyType].GetWrapperCode( $"{pi.DeclaringType.FullName}.{pi.Name}" )}, {setter});";

            return str;
        }

        private static IWrapperObjectCreator[] GetCreators( Type t )
        {
            IEnumerable < BSWConstructorCreatorAttribute > creatorAttributes =
                t.GetCustomAttributes < BSWConstructorCreatorAttribute >();

            List < IWrapperObjectCreator > creators = new List < IWrapperObjectCreator >();

            foreach ( BSWConstructorCreatorAttribute bswConstructorCreatorAttribute in creatorAttributes )
            {
                IWrapperObjectCreator c =
                    ( IWrapperObjectCreator )Activator.CreateInstance(
                                                                      bswConstructorCreatorAttribute.
                                                                          ConstructorCreatorType
                                                                     );

                creators.Add( c );
            }

            return creators.ToArray();
        }

        private static bool IsValidType( Type t, Dictionary < Type, WrapperTypeInfo > wrappers )
        {
            if ( t == typeof( void ) )
            {
                return true;
            }

            if ( !wrappers.ContainsKey( t ) )
            {
                ( string _, string name ) = Generate( t, wrappers );

                return name != "";
            }
            else
            {
                return wrappers[t].GeneratedClass != "";
            }
        }

        #endregion

    }

}

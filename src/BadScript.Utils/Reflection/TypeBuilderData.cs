using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Utils
{

    internal struct TypeBuilderData
    {
        private static readonly Dictionary < Type, TypeBuilderData > s_Builders =
            new Dictionary < Type, TypeBuilderData >();
        private readonly Dictionary < string, TypePropertyBuilderData > m_PropertyData;
        private static readonly Dictionary < string, List<BSFunction>> s_Constructors = new Dictionary < string, List<BSFunction>>();

        internal static ABSTable GetConstructorData()
        {
            Dictionary < ABSObject, ABSObject > d = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, List <BSFunction> > keyValuePair in s_Constructors )
            {
                d[new BSObject( keyValuePair.Key )] = new BSArray( keyValuePair.Value );
            }

            return new BSTable( SourcePosition.Unknown, d );
        }

        private readonly Type m_Type;
        private bool m_Initialized;

        internal static void Expand()
        {

            foreach ( KeyValuePair < Type, TypeBuilderData > typeBuilderData in s_Builders.ToArray() )
            {
                typeBuilderData.Value.Initialize();
            }
        }

        internal static void ExpandAll()
        {
            while ( s_Builders.Any( x => !x.Value.m_Initialized ) )
            {
                Expand();
            }
        }

        public static TypeBuilderData GetData( Type t )
        {
            if ( s_Builders.ContainsKey( t ) )
            {
                return s_Builders[t];
            }

            Console.WriteLine( "Building Data for Type: " + t );

            if ( t.IsArray )
            {
                //Add Array Element
                s_Builders[t.GetElementType()] = GetData( t.GetElementType() );
            }

            return s_Builders[t] = new TypeBuilderData( t );
        }

        private static TypePropertyBuilderData GenerateData( Type t, PropertyInfo pi )
        {
            Type pType = pi.PropertyType;
            TypeBuilderData tData = GetData( t );
            Action < object, ABSObject > setter = null;

            if ( pi.CanWrite )
            {
                setter = ( o, absObject ) =>
                {
                    if ( absObject is BSObject rtype )
                    {
                        pi.SetValue( o, rtype.GetInternalObject() );
                    }
                };
            }

            return new TypePropertyBuilderData( o => tData.WrapObject( pi.GetValue( o ) ), setter );
        }

        private static TypePropertyBuilderData GenerateData( Type t, FieldInfo pi )
        {
            Type pType = pi.FieldType;
            TypeBuilderData tData = GetData( t );
            Action < object, ABSObject > setter = null;

            if ( !pi.IsInitOnly )
            {
                setter = ( o, absObject ) =>
                {
                    if ( absObject is BSObject rtype )
                    {
                        pi.SetValue( o, rtype.GetInternalObject() );
                    }
                };
            }

            return new TypePropertyBuilderData( o => tData.WrapObject( pi.GetValue( o ) ), setter );
        }

        private static TypePropertyBuilderData GenerateData( Type t, MethodInfo mi )
        {
            TypeBuilderData tRet = GetData( mi.ReturnType );

            Func < object, ABSObject > getter = o =>
            {
                Func < ABSObject[], ABSObject > func = objects =>
                {
                    object[] args = new object[objects.Length];

                    for ( int i = 0; i < objects.Length; i++ )
                    {
                        ABSObject absObject = objects[i];

                        if ( absObject is BSObject arg )
                        {
                            args[i] = arg.GetInternalObject();
                        }
                    }
                    return tRet.WrapObject( mi.Invoke( o, args ) );
                };

                ParameterInfo[] pis = mi.GetParameters();
                int min = 0;
                int max = 0;
                foreach ( ParameterInfo parameterInfo in pis )
                {
                    if(!parameterInfo.IsOptional)
                    {
                        min++;
                    }
                    max++;
                }

                BSFunction f = new BSFunction( $"function c#reflectedfunc", func, min, max );

                return f;
            };

            return new TypePropertyBuilderData( getter, null );
        }

        private static TypePropertyBuilderData GenerateData( Type t, ConstructorInfo mi )
        {
            TypeBuilderData tRet = GetData( mi.DeclaringType );

            Func < ABSObject[], ABSObject > func = objects =>
            {
                object[] args = new object[objects.Length];

                for ( int i = 0; i < objects.Length; i++ )
                {
                    ABSObject absObject = objects[i];

                    if ( absObject is BSObject arg )
                    {
                        args[i] = arg.GetInternalObject();
                    }
                }

                return tRet.WrapObject( mi.Invoke( args ) );
            };

            BSFunction f = new BSFunction( $"function {mi.DeclaringType.Name}.ctor", func, mi.GetParameters().Length );

            if ( s_Constructors.ContainsKey( mi.DeclaringType.Name ) )
                s_Constructors[mi.DeclaringType.Name].Add( f );
            else
                s_Constructors[mi.DeclaringType.Name] = new List < BSFunction > { f };
            return new TypePropertyBuilderData( o => f, null );
        }

        private static TypePropertyBuilderData GenerateData( Type t, MemberInfo mi )
        {
            if ( mi is PropertyInfo pi )
            {
                return GenerateData( t, pi );
            }

            if ( mi is FieldInfo fi )
            {
                return GenerateData( t, fi );
            }

            if ( mi is MethodInfo mei )
            {
                return GenerateData( t, mei );
            }

            if ( mi is ConstructorInfo ci )
            {
                return GenerateData( t, ci );
            }


            return TypePropertyBuilderData.Empty;

             }

        private void Initialize()
        {
            if ( m_Initialized )
            {
                return;
            }
            m_Initialized = true;

            MemberInfo[] mis = m_Type.GetMembers();

            foreach ( MemberInfo memberInfo in mis )
            {
                string name = memberInfo.Name;
                if (memberInfo is MethodInfo mi)
                {
                    ParameterInfo[] pis = mi.GetParameters();

                    if ( pis.Length != 0 && !pis.All(x=>x.IsOptional) )
                    {
                        name = name + "_" + mi.GetParameters().Length;
                    }
                }
                if (!m_PropertyData.ContainsKey(name))
                {
                    m_PropertyData[name] = GenerateData(m_Type, memberInfo);
                }
            }

        }

        public TypeBuilderData( Type t )
        {
            m_PropertyData = new Dictionary < string, TypePropertyBuilderData >();

            m_Type = t;

            m_Initialized = false;
        }

        public ABSObject WrapObject( object instance )
        {
            if ( !m_Initialized )
            {
                Initialize();
            }

            Dictionary < string, ABSReference > props = new Dictionary < string, ABSReference >();

            foreach ( KeyValuePair < string, TypePropertyBuilderData > typePropertyBuilderData in m_PropertyData )
            {
                props[typePropertyBuilderData.Key] = new BSReflectionReference(
                    typePropertyBuilderData.Value.MakeGetter( instance ),
                    typePropertyBuilderData.Value.MakeSetter( instance ) );
            }

            return new BSReflectionTypeObject(m_Type, instance, props );
        }
    }

}

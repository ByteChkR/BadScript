using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Utility.Reflection;

namespace BadScript.Utils.Reflection
{

    internal struct TypeBuilderData
    {

        private static readonly List < Type > s_FilterTypes = new List < Type >();
        private static TypeBuilderTypeFilter s_FilterType = TypeBuilderTypeFilter.Blacklist;

        internal static void SetFilterType( TypeBuilderTypeFilter filter )
        {
            s_FilterType = filter;
        }

        internal static void AddFilterType( Type t )
        {
            if ( !s_FilterTypes.Contains( t ) )
            {
                s_FilterTypes.Add( t );
            }
        }

        private static readonly Dictionary < Type, TypeBuilderData > s_Builders =
            new Dictionary < Type, TypeBuilderData >();

        private readonly Dictionary < string, TypePropertyBuilderData > m_PropertyData;

        private static readonly Dictionary < string, (bool locked, List < BSFunction > ctors) > s_Constructors =
            new Dictionary < string, (bool locked, List < BSFunction > ctors) >();

        internal static ABSTable GetConstructorData()
        {
            Dictionary < ABSObject, ABSObject > d = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, (bool locked, List < BSFunction > ctors) > keyValuePair in s_Constructors )
            {
                d[new BSObject( keyValuePair.Key )] = new BSArray( keyValuePair.Value.ctors );
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

            if ( t.IsArray )
            {
                //Add Array Element
                s_Builders[t.GetElementType()] = GetData( t.GetElementType() );
            }

            return s_Builders[t] = new TypeBuilderData( t );
        }

        private static TypePropertyBuilderData GenerateData( Type t, PropertyInfo pi )
        {
            TypeBuilderData tData = GetData( t );
            Action < object, ABSObject > setter = null;

            if ( pi.CanWrite )
            {
                setter = ( o, absObject ) =>
                         {
                             if ( absObject is BSObject rtype )
                             {
                                 object val = Convert.ChangeType( rtype.GetInternalObject(), pi.PropertyType );
                                 pi.SetValue( o, val );
                             }
                         };
            }

            return new TypePropertyBuilderData( o => tData.WrapObject( pi.GetValue( o ) ), setter );
        }

        private static TypePropertyBuilderData GenerateData( Type t, FieldInfo pi )
        {
            TypeBuilderData tData = GetData( t );
            Action < object, ABSObject > setter = null;

            if ( !pi.IsInitOnly )
            {
                setter = ( o, absObject ) =>
                         {
                             if ( absObject is BSObject rtype )
                             {
                                 object val = Convert.ChangeType( rtype.GetInternalObject(), pi.FieldType );

                                 pi.SetValue( o, val );
                             }
                         };
            }

            return new TypePropertyBuilderData( o => tData.WrapObject( pi.GetValue( o ) ), setter );
        }

        private static (int, int) GetParamRange( ParameterInfo[] pis )
        {
            int min = 0;
            int max = 0;

            foreach ( ParameterInfo parameterInfo in pis )
            {
                if ( !parameterInfo.IsOptional )
                {
                    min++;
                }

                max++;
            }

            return ( min, max );
        }

        private static TypePropertyBuilderData GenerateData( MethodInfo mi )
        {
            TypeBuilderData tRet = GetData( mi.ReturnType );

            ParameterInfo[] pis = mi.GetParameters();

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
                                                                args[i] = Convert.ChangeType(
                                                                     arg.GetInternalObject(),
                                                                     pis[i].ParameterType
                                                                    );
                                                            }
                                                        }

                                                        return tRet.WrapObject( mi.Invoke( o, args ) );
                                                    };

                                                    ( int min, int max ) = GetParamRange( pis );

                                                    BSFunction f = new BSFunction(
                                                         GenerateSignature( $"{mi.Name}", pis ),
                                                         func,
                                                         min,
                                                         max
                                                        );

                                                    return f;
                                                };

            return new TypePropertyBuilderData( getter, null );
        }

        private static string GenerateSignature( string fname, ParameterInfo[] parameter )
        {
            StringBuilder sb = new StringBuilder( $"function {fname}(" );

            for ( int i = 0; i < parameter.Length; i++ )
            {
                if ( i == 0 )
                {
                    sb.Append( parameter[i].Name );
                }
                else
                {
                    sb.Append( ", " + parameter[i].Name );
                }
            }

            sb.Append( ")" );

            return sb.ToString();
        }

        private static TypePropertyBuilderData GenerateData( Type t, ConstructorInfo mi )
        {
            TypeBuilderData tRet = GetData( mi.DeclaringType );

            ParameterInfo[] pis = mi.GetParameters();

            Func < ABSObject[], ABSObject > func = objects =>
                                                   {
                                                       object[] args = new object[objects.Length];

                                                       for ( int i = 0; i < objects.Length; i++ )
                                                       {
                                                           ABSObject absObject = objects[i];

                                                           if ( absObject is BSObject arg )
                                                           {
                                                               args[i] = Convert.ChangeType(
                                                                    arg.GetInternalObject(),
                                                                    pis[i].ParameterType
                                                                   );
                                                           }
                                                       }

                                                       return tRet.WrapObject( mi.Invoke( args ) );
                                                   };

            ( int min, int max ) = GetParamRange( pis );

            BSFunction f = new BSFunction( GenerateSignature( $"{mi.DeclaringType.Name}.ctor", pis ), func, min, max );

            if ( s_Constructors.ContainsKey( mi.DeclaringType.Name ) && !s_Constructors[mi.DeclaringType.Name].locked )
            {
                s_Constructors[mi.DeclaringType.Name].ctors.Add( f );
            }
            else
            {
                s_Constructors[mi.DeclaringType.Name] = ( false, new List < BSFunction > { f } );
            }

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
            if ( m_Initialized ||
                 s_FilterType == TypeBuilderTypeFilter.Blacklist && s_FilterTypes.Contains( m_Type ) ||
                 s_FilterType == TypeBuilderTypeFilter.Whitelist && !s_FilterTypes.Contains( m_Type ) )
            {
                return;
            }

            m_Initialized = true;

            MemberInfo[] mis = m_Type.GetMembers();

            foreach ( MemberInfo memberInfo in mis )
            {
                string name = memberInfo.Name;

                if ( memberInfo is MethodInfo mi )
                {
                    ParameterInfo[] pis = mi.GetParameters();

                    if ( pis.Length != 0 && !pis.All( x => x.IsOptional ) )
                    {
                        name = name + "_" + mi.GetParameters().Length;
                    }
                }

                if ( !m_PropertyData.ContainsKey( name ) || name == ".ctor" )
                {
                    m_PropertyData[name] = GenerateData( m_Type, memberInfo );
                }
            }

            (bool locked, List < BSFunction > ctors ) e = s_Constructors[m_Type.Name];
            e.locked = true;
            s_Constructors[m_Type.Name] = e;
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
                                                                               typePropertyBuilderData.Value.MakeGetter(
                                                                                    instance
                                                                                   ),
                                                                               typePropertyBuilderData.Value.MakeSetter(
                                                                                    instance
                                                                                   )
                                                                              );
            }

            return new BSReflectionTypeObject( m_Type, instance, props );
        }

    }

}

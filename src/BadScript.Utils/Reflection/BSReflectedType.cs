using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Utility.Reflection;

namespace BadScript.Utils.Reflection
{

    public class BSReflectedType : ABSObject
    {

        private readonly Type m_Type;

        private readonly Dictionary < string, ABSReference > m_StaticMembers;
        private readonly ConstructorInfo[] m_Constructors;

        public override bool IsNull => false;

        #region Public

        public BSReflectedType( Type t ) : base( SourcePosition.Unknown )
        {
            m_Type = t;
            m_StaticMembers = new Dictionary < string, ABSReference >();
            m_Constructors = m_Type.GetConstructors();

            MemberInfo[] smis = m_Type.GetMembers( BindingFlags.Public | BindingFlags.Static );
            Populate( m_StaticMembers, smis, null );
        }

        public static ConstructorInfo FindConstructor( ConstructorInfo[] mis, object[] args )
        {
            foreach ( ConstructorInfo methodInfo in mis )
            {
                ParameterInfo[] pis = methodInfo.GetParameters();
                int min = 0;
                int max = pis.Length;

                foreach ( ParameterInfo parameterInfo in pis )
                {
                    if ( parameterInfo.IsOptional )
                    {
                        break;
                    }
                    else
                    {
                        min++;
                    }
                }

                if ( min <= args.Length && max >= args.Length && CheckTypes( pis, args ) )
                {
                    return methodInfo;
                }
            }

            throw new Exception(); //TODO: Do better
        }

        public ABSObject Create( ABSObject[] args )
        {
            object[] objs = BSReflectedMethod.GetObjects( args );

            return BSReflectionInterface.Instance.Wrap( FindConstructor( m_Constructors, objs ).Invoke( objs ) );
        }

        public override bool Equals( ABSObject other )
        {
            return other is BSReflectedType rt && rt.m_Type == m_Type;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return m_StaticMembers[propertyName];
        }

        public override bool HasProperty( string propertyName )
        {
            return m_StaticMembers.ContainsKey( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new NotSupportedException( "Types can not be invoked." );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            if ( doneList.ContainsKey( this ) )
            {
                return "<recursion>";
            }

            doneList[this] = "{}";

            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter( sw );
            tw.WriteLine( '{' );

            foreach ( KeyValuePair < string, ABSReference > bsRuntimeObject in m_StaticMembers )
            {
                List < string > keyLines = bsRuntimeObject.Key.
                                                           Split(
                                                                 new[] { '\n' },
                                                                 StringSplitOptions.RemoveEmptyEntries
                                                                ).
                                                           Select( x => x.Trim() ).
                                                           Where( x => !string.IsNullOrEmpty( x ) ).
                                                           ToList();

                List < string > valueLines;

                if ( BSReflectionInterface.Instance.IsRecursionSafe( bsRuntimeObject.Value.ResolveReference() ) )
                {
                    valueLines = bsRuntimeObject.Value.SafeToString( doneList ).
                                                 Split(
                                                       new[] { '\n' },
                                                       StringSplitOptions.RemoveEmptyEntries
                                                      ).
                                                 Select( x => x.Trim() ).
                                                 Where( x => !string.IsNullOrEmpty( x ) ).
                                                 ToList();
                }
                else
                {
                    valueLines = new List < string > { bsRuntimeObject.Value.ToString() };
                }

                tw.Indent = 1;

                for ( int i = 0; i < keyLines.Count; i++ )
                {
                    string keyLine = keyLines[i];

                    if ( i < keyLines.Count - 1 )
                    {
                        tw.WriteLine( keyLine );
                    }
                    else
                    {
                        tw.Write( keyLine + " = " );
                    }
                }

                tw.Indent = 2;

                for ( int i = 0; i < valueLines.Count; i++ )
                {
                    string valueLine = valueLines[i];
                    tw.WriteLine( valueLine );
                }
            }

            tw.Indent = 0;
            tw.WriteLine( '}' );

            doneList[this] = sw.ToString();

            return doneList[this];
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new NotSupportedException( "Can not Set Properties on reflected type directly." );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = null;

            return false;
        }

        public BSReflectedObject Wrap( object o )
        {
            if ( o.GetType() != m_Type )
            {
                throw new Exception();
            }

            Dictionary < string, ABSReference > members = new Dictionary < string, ABSReference >();

            MemberInfo[] mis = m_Type.GetMembers( BindingFlags.Public | BindingFlags.Instance );
            Populate( members, mis, o );

            return new BSReflectedObject( members, o );
        }

        #endregion

        #region Private

        private static bool CheckTypes( ParameterInfo[] pis, object[] args )
        {
            for ( int i = 0; i < args.Length; i++ )
            {
                Type t = pis[i].ParameterType;
                Type a = args[i].GetType();

                if ( !t.IsAssignableFrom( a ) )
                {
                    return false;
                }
            }

            return true;
        }

        private static void Populate( Dictionary < string, ABSReference > members, MemberInfo[] mis, object instance )
        {
            Dictionary < string, List < MethodInfo > > methods = new Dictionary < string, List < MethodInfo > >();

            foreach ( MemberInfo memberInfo in mis )
            {
                if ( memberInfo is FieldInfo fi )
                {
                    members[fi.Name] = new BSReflectionReference(
                                                                 () => BSReflectionInterface.Instance.Wrap(
                                                                      fi.GetValue( instance )
                                                                     ),
                                                                 o =>
                                                                 {
                                                                     if ( o is IBSWrappedObject wo )
                                                                     {
                                                                         fi.SetValue(
                                                                              instance,
                                                                              wo.GetInternalObject()
                                                                             );
                                                                     }
                                                                     else
                                                                     {
                                                                         throw new Exception();
                                                                     }
                                                                 }
                                                                );
                }
                else if ( memberInfo is PropertyInfo pi )
                {
                    members[pi.Name] = new BSReflectionReference(
                                                                 () => BSReflectionInterface.Instance.Wrap(
                                                                      pi.GetValue( instance )
                                                                     ),
                                                                 o =>
                                                                 {
                                                                     if ( o is IBSWrappedObject wo )
                                                                     {
                                                                         pi.SetValue(
                                                                              instance,
                                                                              wo.GetInternalObject()
                                                                             );
                                                                     }
                                                                     else
                                                                     {
                                                                         throw new Exception();
                                                                     }
                                                                 }
                                                                );
                }
                else if ( memberInfo is MethodInfo mi )
                {
                    if ( methods.ContainsKey( mi.Name ) )
                    {
                        methods[mi.Name].Add( mi );
                    }
                    else
                    {
                        methods[mi.Name] = new List < MethodInfo > { mi };
                    }
                }
            }

            foreach ( KeyValuePair < string, List < MethodInfo > > keyValuePair in methods )
            {
                members[keyValuePair.Key] =
                    new BSFunctionReference( new BSReflectedMethod( instance, keyValuePair.Value.ToArray() ) );
            }
        }

        #endregion

    }

}

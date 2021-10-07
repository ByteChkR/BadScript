using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Reflection
{

    public class BSReflectedObject : ABSObject, IBSWrappedObject
    {

        private readonly Dictionary < string, ABSReference > m_Members;
        private readonly object m_Instance;

        public override bool IsNull() => m_Instance == null;

        protected override int GetHashCodeImpl()
        {
            return m_Instance?.GetHashCode() ?? 0;
        }
        #region Public

        public BSReflectedObject( Dictionary < string, ABSReference > members, object instance ) :
            base( SourcePosition.Unknown )
        {
            m_Members = members;
            m_Instance = instance;
        }

        public static bool IsNumericType( object o )
        {
            switch ( Type.GetTypeCode( o.GetType() ) )
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;

                default:
                    return false;
            }
        }

        public override bool Equals( ABSObject other )
        {
            return other is BSReflectedObject ro && ro.m_Instance == m_Instance;
        }

        public object GetInternalObject()
        {
            return m_Instance;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return m_Members[propertyName];
        }

        public override bool HasProperty( string propertyName )
        {
            return m_Members.ContainsKey( propertyName );
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

            foreach ( KeyValuePair < string, ABSReference > bsRuntimeObject in m_Members )
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

        public override string ToString()
        {
            return m_Instance.ToString();
        }

        public override bool TryConvertBool( out bool v )
        {
            if ( m_Instance is bool b )
            {
                v = b;

                return true;
            }

            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            if ( IsNumericType() )
            {
                d = ( decimal )Convert.ChangeType( m_Instance, TypeCode.Decimal );

                return true;
            }

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            if ( m_Instance is string s )
            {
                v = s;

                return true;
            }

            v = null;

            return false;
        }

        #endregion

        #region Private

        private bool IsNumericType()
        {
            return IsNumericType( m_Instance );
        }

        #endregion

    }

}

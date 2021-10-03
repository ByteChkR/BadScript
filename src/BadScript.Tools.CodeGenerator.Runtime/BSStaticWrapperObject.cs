using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Tools.CodeGenerator.Runtime
{

    public class BSStaticWrapperObject : ABSObject, IEnumerable < IForEachIteration >
    {

        protected Dictionary < string, ABSReference > m_StaticProperties;
        private Type m_WrappedType;

        public string[] Properties => m_StaticProperties.Keys.ToArray();

        public override bool IsNull => false;

        #region Public

        public BSStaticWrapperObject( Type t ) : base( SourcePosition.Unknown )
        {
            m_WrappedType = t;
            m_StaticProperties = new Dictionary < string, ABSReference >();
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            foreach ( KeyValuePair < string, ABSReference > keyValuePair in m_StaticProperties )
            {
                yield return new ForEachIteration( new ABSObject[] { new BSObject( keyValuePair.Key ) } );
            }
        }

        public override ABSReference GetProperty( string propertyName )
        {
            if ( m_StaticProperties.ContainsKey( propertyName ) )
            {
                return m_StaticProperties[propertyName];
            }

            throw new BSRuntimeException( "Invalid Property Name: " + propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_StaticProperties.ContainsKey( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( "Can not Invoke Object" );
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

            foreach ( KeyValuePair < string, ABSReference > bsRuntimeObject in m_StaticProperties )
            {
                List < string > keyLines = bsRuntimeObject.Key.
                                                           Split(
                                                                 new[] { '\n' },
                                                                 StringSplitOptions.RemoveEmptyEntries
                                                                ).
                                                           Select( x => x.Trim() ).
                                                           Where( x => !string.IsNullOrEmpty( x ) ).
                                                           ToList();

                List < string > valueLines = new List < string >();

                if ( WrapperHelper.AllowRecurseToString )
                {
                    ABSObject resolvedValue = bsRuntimeObject.Value.ResolveReference();

                    if ( resolvedValue is IBSWrappedObject wo )
                    {
                        valueLines = wo.GetInternalObject().ToString().Split( '\n' ).ToList();
                    }
                    else
                    {
                        valueLines = resolvedValue.SafeToString( doneList ).
                                                   Split(
                                                         new[] { '\n' },
                                                         StringSplitOptions.RemoveEmptyEntries
                                                        ).
                                                   Select( x => x.Trim() ).
                                                   Where( x => !string.IsNullOrEmpty( x ) ).
                                                   ToList();
                    }
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
                        if ( valueLines.Count != 0 )
                        {
                            tw.Write( keyLine + " = " );
                        }
                        else
                        {
                            tw.WriteLine( keyLine + " = " );
                        }
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

            return sw.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            if ( m_StaticProperties.ContainsKey( propertyName ) )
            {
                m_StaticProperties[propertyName].Assign( obj );

                return;
            }

            throw new BSRuntimeException( "Object does not support writing properties" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = true;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = m_WrappedType.Name;

            return true;
        }

        #endregion

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}

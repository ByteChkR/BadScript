﻿using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Common.Types.Implementations
{

    public class BSTable : ABSTable, IEnumerable < IForEachIteration >
    {
        private readonly Dictionary < ABSObject, ABSObject > m_InnerTable =
            new Dictionary < ABSObject, ABSObject >();

        public override bool IsNull => false;

        public override ABSArray Keys => new BSArray( m_InnerTable.Keys );

        public override ABSArray Values => new BSArray( m_InnerTable.Values );

        private bool m_Locked;

        #region Public

        public BSTable()
        {
        }

        public BSTable( Dictionary < ABSObject, ABSObject > startObjects )
        {
            m_InnerTable = startObjects;
        }

        public void Lock()
        {
            m_Locked = true;
        }

        public override void Clear()
        {
            m_InnerTable.Clear();
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetElement( ABSObject i )
        {
            return new BSTableReference( this, i, m_Locked );
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            foreach ( KeyValuePair < ABSObject, ABSObject > keyValuePair in m_InnerTable )
            {
                yield return new ForEachIteration( new[] { keyValuePair.Key, keyValuePair.Value } );
            }
        }

        public override int GetLength()
        {
            return m_InnerTable.Count;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            ABSObject k = new BSObject( propertyName );

            if ( HasElement( k ) )
            {
                return GetElement( k );
            }

            return new BSTableReference( this, k, m_Locked );

            //throw new BSRuntimeException( $"Property {propertyName} does not exist" );
        }

        public override ABSObject GetRawElement( ABSObject k )
        {
            ABSObject key = m_InnerTable.Keys.FirstOrDefault( x => x.Equals( k ) );

            return key != null ? m_InnerTable[key] : new BSObject( null );
        }

        public override bool HasElement( ABSObject i )
        {
            ABSObject key = m_InnerTable.Keys.FirstOrDefault( x => x.Equals( i ) ) ?? i;

            return m_InnerTable.ContainsKey( key );
        }

        public override bool HasProperty( string propertyName )
        {
            return HasElement( new BSObject( propertyName ) );
        }

        public override void InsertElement( ABSObject k, ABSObject o )
        {
            ABSObject key = m_InnerTable.Keys.FirstOrDefault( x => x.Equals( k ) ) ?? k;
            m_InnerTable[key] = o;
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( $"Can not invoke '{this}'" );
        }

        public override void RemoveElement( ABSObject k )
        {
            ABSObject key = m_InnerTable.Keys.First( x => x.Equals( k ) );
            m_InnerTable.Remove( key );
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

            foreach ( KeyValuePair < ABSObject, ABSObject > bsRuntimeObject in m_InnerTable )
            {
                List < string > keyLines = bsRuntimeObject.Key.SafeToString( doneList ).
                                                           Split(
                                                               new[] { '\n' },
                                                               StringSplitOptions.RemoveEmptyEntries
                                                           ).
                                                           Select( x => x.Trim() ).
                                                           Where( x => !string.IsNullOrEmpty( x ) ).
                                                           ToList();

                List < string > valueLines = bsRuntimeObject.Value.SafeToString( doneList ).
                                                             Split(
                                                                 new[] { '\n' },
                                                                 StringSplitOptions.RemoveEmptyEntries
                                                             ).
                                                             Select( x => x.Trim() ).
                                                             Where( x => !string.IsNullOrEmpty( x ) ).
                                                             ToList();

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
            InsertElement( new BSObject( propertyName ), obj );
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
            v = null;

            return false;
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

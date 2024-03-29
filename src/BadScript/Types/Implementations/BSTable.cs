﻿using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Types.References;
using BadScript.Types.References.Implementations;

namespace BadScript.Types.Implementations
{

    public sealed class BSTable : ABSTable, IEnumerable < IForEachIteration >
    {

        private readonly Dictionary < ABSObject, ABSObject > m_InnerTable =
            new Dictionary < ABSObject, ABSObject >();

        private bool m_Locked;

        public override ABSArray Keys => new BSArray( m_InnerTable.Keys );

        public override ABSArray Values => new BSArray( m_InnerTable.Values );

        #region Public

        public BSTable( SourcePosition pos ) : base( pos )
        {
        }

        public BSTable( SourcePosition pos, Dictionary < ABSObject, ABSObject > startObjects ) : base( pos )
        {
            m_InnerTable = startObjects;
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

        public override int GetLength()
        {
            return m_InnerTable.Count;
        }

        public ABSReference GetProperty( string propertyName, bool lockRef )
        {
            ABSObject k = new BSObject( propertyName );

            if ( HasElement( k ) )
            {
                return GetElement( k );
            }

            return new BSTableReference( this, k, lockRef );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return GetProperty( propertyName, m_Locked );
        }

        public override ABSObject GetRawElement( ABSObject k )
        {
            ABSObject key = k.ResolveReference();

            return m_InnerTable[key];
        }

        public override bool HasElement( ABSObject i )
        {
            ABSObject key = i.ResolveReference();

            return m_InnerTable.ContainsKey( key );
        }

        public override bool HasProperty( string propertyName )
        {
            return HasElement( new BSObject( propertyName ) );
        }

        public override void InsertElement( ABSObject k, ABSObject o )
        {
            ABSObject key = k.ResolveReference();

            if ( m_InnerTable.TryGetValue( key, out ABSObject val ) && val is ABSReference reference )
            {
                reference.Assign( o.ResolveReference() );
            }
            else
            {
                m_InnerTable[key] = o.ResolveReference();
            }
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( Position, $"Can not invoke '{this}'" );
        }

        public override bool IsNull()
        {
            return false;
        }

        public void Lock()
        {
            m_Locked = true;
        }

        public override void Remove( ABSObject k )
        {
            m_InnerTable.Remove( k.ResolveReference() );
        }

        public override void RemoveElement( ABSObject k )
        {
            m_InnerTable.Remove( k.ResolveReference() );
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
                                                           Where( x => !string.IsNullOrEmpty( x ) ).
                                                           ToList();

                List < string > valueLines = bsRuntimeObject.Value.SafeToString( doneList ).
                                                             Split(
                                                                   new[] { '\n' },
                                                                   StringSplitOptions.RemoveEmptyEntries
                                                                  ).
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

        public override void SetRawElement( ABSObject k, ABSObject o )
        {
            ABSObject key = k.ResolveReference();
            m_InnerTable[key] = o;
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
            
            if (HasProperty("ToString"))
            {
                v = GetProperty("ToString").Invoke(Array.Empty<ABSObject>()).ConvertString();
                return true;
            }
            v = null;

            return false;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_InnerTable.GetHashCode();
        }

        #endregion

        #region Private

        IEnumerator < IForEachIteration > IEnumerable < IForEachIteration >.GetEnumerator()
        {
            foreach ( KeyValuePair < ABSObject, ABSObject > keyValuePair in m_InnerTable )
            {
                yield return new ForEachIteration( new[] { keyValuePair.Key, keyValuePair.Value } );
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}

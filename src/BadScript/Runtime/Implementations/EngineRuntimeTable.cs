using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BadScript.Runtime.Implementations
{

    public class EngineRuntimeTable : BSRuntimeTable
    {

        private readonly Dictionary < BSRuntimeObject, BSRuntimeObject > m_InnerTable =
            new Dictionary < BSRuntimeObject, BSRuntimeObject >();

        public override BSRuntimeArray Keys => new EngineRuntimeArray( m_InnerTable.Keys );

        public override BSRuntimeArray Values => new EngineRuntimeArray( m_InnerTable.Values );

        #region Public

        public EngineRuntimeTable()
        {
        }

        public EngineRuntimeTable( Dictionary < BSRuntimeObject, BSRuntimeObject > startObjects )
        {
            m_InnerTable = startObjects;
        }

        public override void Clear()
        {
            m_InnerTable.Clear();
        }

        public override bool Equals( BSRuntimeObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override BSRuntimeReference GetElement( BSRuntimeObject i )
        {
            return new BSRuntimeTableReference( this, i );
        }

        public override int GetLength()
        {
            return m_InnerTable.Count;
        }

        public override BSRuntimeReference GetProperty( string propertyName )
        {
            BSRuntimeObject k = new EngineRuntimeObject( propertyName );

            if ( HasElement( k ) )
            {
                return GetElement( k );
            }

            throw new Exception( $"Property {propertyName} does not exist" );
        }

        public override BSRuntimeObject GetRawElement( BSRuntimeObject k )
        {
            BSRuntimeObject key = m_InnerTable.Keys.FirstOrDefault( x => x.Equals( k ) );

            return key !=null?m_InnerTable[key]:new EngineRuntimeObject(null);
        }

        public override bool HasElement( BSRuntimeObject i )
        {
            BSRuntimeObject key = m_InnerTable.Keys.FirstOrDefault( x => x.Equals( i ) ) ?? i;

            return m_InnerTable.ContainsKey( key );
        }

        public override bool HasProperty( string propertyName )
        {
            return HasElement( new EngineRuntimeObject( propertyName ) );
        }

        public override void InsertElement( BSRuntimeObject k, BSRuntimeObject o )
        {
            BSRuntimeObject key = m_InnerTable.Keys.FirstOrDefault( x => x.Equals( k ) ) ?? k;
            m_InnerTable[key] = o;
        }

        public override BSRuntimeObject Invoke( BSRuntimeObject[] args )
        {
            throw new Exception( $"Can not invoke '{this}'" );
        }

        public override void RemoveElement( BSRuntimeObject k )
        {
            BSRuntimeObject key = m_InnerTable.Keys.First( x => x.Equals( k ) );
            m_InnerTable.Remove( key );
        }

        public override string SafeToString( Dictionary < BSRuntimeObject, string > doneList )
        {
            if ( doneList.ContainsKey( this ) )
            {
                return "<recursion>";
            }

            doneList[this] = "{}";

            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter( sw );
            tw.WriteLine( '{' );

            foreach ( KeyValuePair < BSRuntimeObject, BSRuntimeObject > bsRuntimeObject in m_InnerTable )
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

        public override void SetProperty( string propertyName, BSRuntimeObject obj )
        {
            InsertElement( new EngineRuntimeObject( propertyName ), obj );
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

    }

}

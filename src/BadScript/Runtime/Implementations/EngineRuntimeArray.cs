using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BadScript.Runtime.Implementations
{

    public class EngineRuntimeArray : BSRuntimeArray
    {

        private readonly List < BSRuntimeObject > m_InnerArray;

        #region Public

        public EngineRuntimeArray()
        {
            m_InnerArray = new List < BSRuntimeObject >();
        }

        public EngineRuntimeArray( int capacity )
        {
            m_InnerArray = new List < BSRuntimeObject >( capacity );
        }

        public EngineRuntimeArray( IEnumerable < BSRuntimeObject > collection )
        {
            m_InnerArray = new List < BSRuntimeObject >( collection );
        }

        public override bool Equals( BSRuntimeObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override BSRuntimeReference GetElement( int i )
        {
            return new BSRuntimeArrayReference( this, i );

            ;
        }

        public override int GetLength()
        {
            return m_InnerArray.Count;
        }

        public override BSRuntimeReference GetProperty( string propertyName )
        {
            throw new Exception( $"Property {propertyName} does not exist" );
        }

        public override BSRuntimeObject GetRawElement( int i )
        {
            if ( m_InnerArray.Count <= i || i < 0)
                return new EngineRuntimeObject( null );
            return m_InnerArray[i];
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override void InsertElement( int i, BSRuntimeObject o )
        {
            m_InnerArray.Insert( i, o );
        }

        public override BSRuntimeObject Invoke( BSRuntimeObject[] args )
        {
            throw new Exception( $"Can not invoke '{this}'" );
        }

        public override void RemoveElement( int i )
        {
            m_InnerArray.RemoveAt( i );
        }

        public override string SafeToString( Dictionary < BSRuntimeObject, string > doneList )
        {
            if ( doneList.ContainsKey( this ) )
            {
                return "<recursion>";
            }

            doneList[this] = "[]";

            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter( sw );
            tw.WriteLine( '[' );
            tw.Indent = 1;

            for ( int i = 0; i < m_InnerArray.Count; i++ )
            {
                BSRuntimeObject bsRuntimeObject = m_InnerArray[i];

                List < string > keyLines =
                    new List < string >( bsRuntimeObject.SafeToString( doneList ).Split( '\n' ) ).
                        Select( x => x.Trim() ).
                        Where( x => !string.IsNullOrEmpty( x ) ).
                        ToList();

                tw.Indent = 1;
                tw.Write( $"{i} = " );

                tw.Indent = 2;

                for ( int j = 0; j < keyLines.Count; j++ )
                {
                    string keyLine = keyLines[j];

                    tw.WriteLine( keyLine );
                }
            }

            tw.Indent = 0;
            tw.WriteLine( ']' );

            doneList[this] = sw.ToString();

            return doneList[this];
        }

        public override void SetElement( int k, BSRuntimeObject o )
        {
            m_InnerArray[k] = o;
        }

        public override void SetProperty( string propertyName, BSRuntimeObject obj )
        {
            throw new Exception( $"Property {propertyName} does not exist" );
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

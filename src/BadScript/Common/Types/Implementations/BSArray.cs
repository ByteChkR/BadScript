using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;

namespace BadScript.Common.Types.Implementations
{

    public class BSArray : ABSArray, IEnumerable < IForEachIteration >
    {
        private readonly List < ABSObject > m_InnerArray;

        public override bool IsNull => false;

        #region Public

        public BSArray()
        {
            m_InnerArray = new List < ABSObject >();
        }

        public BSArray( int capacity )
        {
            m_InnerArray = new List < ABSObject >( capacity );
        }

        public BSArray( IEnumerable < ABSObject > collection )
        {
            m_InnerArray = new List < ABSObject >( collection );
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetElement( int i )
        {
            return new BSArrayReference( this, i );

            ;
        }

        public IEnumerator < IForEachIteration > GetEnumerator()
        {
            ABSObject[] o = new ABSObject[1];

            foreach ( ABSObject absObject in m_InnerArray )
            {
                o[0] = absObject;

                yield return new ForEachIteration( o );
            }
        }

        public override int GetLength()
        {
            return m_InnerArray.Count;
        }

        public override ABSReference GetProperty( string propertyName )
        {
            throw new Exception( $"Property {propertyName} does not exist" );
        }

        public override ABSObject GetRawElement( int i )
        {
            if ( m_InnerArray.Count <= i || i < 0 )
            {
                return new BSObject( null );
            }

            return m_InnerArray[i];
        }

        public override bool HasProperty( string propertyName )
        {
            return false;
        }

        public override void InsertElement( int i, ABSObject o )
        {
            m_InnerArray.Insert( i, o );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new Exception( $"Can not invoke '{this}'" );
        }

        public override void RemoveElement( int i )
        {
            m_InnerArray.RemoveAt( i );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
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
                ABSObject bsObject = m_InnerArray[i];

                List < string > keyLines =
                    new List < string >( bsObject.SafeToString( doneList ).Split( '\n' ) ).
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

        public override void SetElement( int k, ABSObject o )
        {
            m_InnerArray[k] = o;
        }

        public override void SetProperty( string propertyName, ABSObject obj )
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

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

}

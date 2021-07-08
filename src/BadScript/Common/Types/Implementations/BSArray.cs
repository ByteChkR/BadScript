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

    public class BSArray : ABSArray, IEnumerable < IForEachIteration >
    {
        private bool m_Locked = false;
        private readonly Dictionary < string, BSFunction > m_Functions;

        private readonly List < ABSObject > m_InnerArray;

        public override bool IsNull => false;

        #region Public

        public BSArray() : this( new List < ABSObject >() )
        {
        }

        public BSArray( int capacity ) : this( new List < ABSObject >( capacity ) )
        {
        }

        public BSArray( IEnumerable < ABSObject > collection ) : this( new List < ABSObject >( collection ) )
        {
        }

        public void Lock()
        {
            m_Locked = true;
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetElement( int i )
        {
            return new BSArrayReference( this, i, m_Locked);
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
            if ( !m_Functions.ContainsKey( propertyName ) )
            {
                throw new BSRuntimeException( $"Property {propertyName} does not exist" );
            }

            return new BSFunctionReference( m_Functions[propertyName] );
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
            return m_Functions.ContainsKey(propertyName);
        }

        public override void InsertElement( int i, ABSObject o )
        {
            m_InnerArray.Insert( i, o );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( $"Can not invoke '{this}'" );
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
            throw new BSRuntimeException( $"Property {propertyName} does not exist" );
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

        private BSArray( List < ABSObject > o )
        {
            m_InnerArray = o;

            m_Functions = new Dictionary < string, BSFunction >();

            m_Functions["clear"] = new BSFunction(
                "function clear()",
                objects =>
                {
                    m_InnerArray.Clear();

                    return new BSObject( null );
                },
                0 );

            m_Functions["size"] = new BSFunction(
                "function size()",
                objects => new BSObject( ( decimal ) m_InnerArray.Count ),
                0 );

            m_Functions["add"] = new BSFunction(
                "function add(obj0, obj1, obj2, ...)",
                objects =>
                {
                    m_InnerArray.AddRange( objects );

                    return new BSObject( null );
                },
                1,
                int.MaxValue );

            m_Functions["remove"] = new BSFunction(
                "function remove(obj0, obj1, obj2, ...)",
                objects =>
                {
                    foreach ( ABSObject absObject in objects )
                    {
                        m_InnerArray.Remove( absObject );
                    }

                    return new BSObject( null );
                },
                1,
                int.MaxValue );

            m_Functions["removeAt"] = new BSFunction(
                "function removeAt(index0, index1, index2, ...)",
                objects =>
                {
                    foreach ( ABSObject absObject in objects )
                    {
                        m_InnerArray.RemoveAt( ( int ) absObject.ConvertDecimal() );
                    }

                    return new BSObject( null );
                },
                1,
                int.MaxValue );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

}

using System;
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

    public sealed class BSArray : ABSArray, IEnumerable < IForEachIteration >
    {

        private bool m_Locked = false;
        private readonly Dictionary < string, BSFunction > m_Functions;

        private readonly List < ABSObject > m_InnerArray;

        public ABSObject[] Elements => m_InnerArray.ToArray();

        #region Public

        public BSArray( SourcePosition pos ) : this( pos, new List < ABSObject >() )
        {
        }

        public BSArray( SourcePosition pos, int capacity ) : this( new List < ABSObject >( capacity ) )
        {
        }

        public BSArray( SourcePosition pos, IEnumerable < ABSObject > collection ) : this( pos, collection.ToList() )
        {
        }

        public BSArray() : this( SourcePosition.Unknown, new List < ABSObject >() )
        {
        }

        public BSArray( int capacity ) : this( SourcePosition.Unknown, new List < ABSObject >( capacity ) )
        {
        }

        public BSArray( IEnumerable < ABSObject > collection ) : this( SourcePosition.Unknown, collection )
        {
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override IEnumerable < T > ForEach < T >( Func < ABSObject, T > o )
        {
            foreach ( ABSObject absObject in m_InnerArray )
            {
                yield return o( absObject );
            }
        }

        public override ABSReference GetElement( int i )
        {
            return new BSArrayReference( this, i, m_Locked );
        }

        IEnumerator < IForEachIteration > IEnumerable<IForEachIteration>.GetEnumerator()
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
                throw new BSRuntimeException(
                                             Position,
                                             $"Property {propertyName} does not exist in Array {SafeToString()}"
                                            );
            }

            return new BSFunctionReference( m_Functions[propertyName] );
        }

        public override ABSObject GetRawElement( int i )
        {
            if ( m_InnerArray.Count <= i || i < 0 )
            {
                return BSObject.Null;
            }

            return m_InnerArray[i];
        }

        public override bool HasProperty( string propertyName )
        {
            return m_Functions.ContainsKey( propertyName );
        }

        public override void InsertElement( int i, ABSObject o )
        {
            m_InnerArray.Insert( i, o.ResolveReference() );
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
            throw new BSRuntimeException(
                                         Position,
                                         $"Property {propertyName} does not exist in array {SafeToString()}"
                                        );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = false;

            return false;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = decimal.Zero;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = null;

            return false;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_InnerArray.GetHashCode();
        }

        #endregion

        #region Private

        private BSArray( SourcePosition pos, List < ABSObject > o ) : base( pos )
        {
            m_InnerArray = o;

            m_Functions = new Dictionary < string, BSFunction >();

            m_Functions["Clear"] = new BSFunction(
                                                  "function Clear()",
                                                  objects =>
                                                  {
                                                      m_InnerArray.Clear();

                                                      return BSObject.Null;
                                                  },
                                                  0
                                                 );

            m_Functions["Size"] = new BSFunction(
                                                 "function Size()",
                                                 objects => new BSObject( ( decimal )m_InnerArray.Count ),
                                                 0
                                                );

            m_Functions["Add"] = new BSFunction(
                                                "function Add(obj0, obj1, obj2, ...)",
                                                objects =>
                                                {
                                                    m_InnerArray.AddRange(
                                                                          objects.Select( x => x.ResolveReference() )
                                                                         );

                                                    return BSObject.Null;
                                                },
                                                1,
                                                int.MaxValue
                                               );

            m_Functions["Insert"] = new BSFunction(
                                                   "function Insert(idx, obj0, obj1, obj2, ...)",
                                                   objects =>
                                                   {
                                                       m_InnerArray.InsertRange(
                                                                                ( int )objects.First().ConvertDecimal(),
                                                                                objects.Skip( 1 ).
                                                                                    Select( x => x.ResolveReference() )
                                                                               );

                                                       return BSObject.Null;
                                                   },
                                                   1,
                                                   int.MaxValue
                                                  );

            m_Functions["Remove"] = new BSFunction(
                                                   "function Remove(obj0, obj1, obj2, ...)",
                                                   objects =>
                                                   {
                                                       foreach ( ABSObject absObject in objects )
                                                       {
                                                           m_InnerArray.Remove( absObject.ResolveReference() );
                                                       }

                                                       return BSObject.Null;
                                                   },
                                                   1,
                                                   int.MaxValue
                                                  );

            m_Functions["RemoveAt"] = new BSFunction(
                                                     "function RemoveAt(index0, index1, index2, ...)",
                                                     objects =>
                                                     {
                                                         foreach ( ABSObject absObject in objects )
                                                         {
                                                             m_InnerArray.RemoveAt( ( int )absObject.ConvertDecimal() );
                                                         }

                                                         return BSObject.Null;
                                                     },
                                                     1,
                                                     int.MaxValue
                                                    );

            m_Functions["Swap"] = new BSFunction(
                                                 "function Swap(idx1, idx2)",
                                                 objects =>
                                                 {
                                                     int i0 = ( int )objects[0].ConvertDecimal();
                                                     int i1 = ( int )objects[1].ConvertDecimal();
                                                     ABSObject o0 = m_InnerArray[i0];

                                                     m_InnerArray[i0] =
                                                         m_InnerArray[i1];

                                                     m_InnerArray[i1] = o0;

                                                     return BSObject.Null;
                                                 },
                                                 2
                                                );

            m_Functions["Reverse"] = new BSFunction(
                                                    "function Reverse()",
                                                    objects =>
                                                    {
                                                        m_InnerArray.Reverse();

                                                        return BSObject.Null;
                                                    },
                                                    0
                                                   );

            m_Functions["Contains"] = new BSFunction("function Contains(item)", ArrayContains, 1);

            m_Functions["ContentEquals"] = new BSFunction( "function ContentEquals(array)", ArrayContentEquals, 1 );
        }

        private ABSObject ArrayContains(ABSObject[] arg)
        {
            return m_InnerArray.Contains(arg[0].ResolveReference())? BSObject.True : BSObject.False;
        }

        private ABSObject ArrayContentEquals( ABSObject[] arg )
        {
            BSArray a = ( BSArray )arg[0].ResolveReference();

            if ( a.m_InnerArray.Count != m_InnerArray.Count )
            {
                return BSObject.False;
            }

            for ( int i = 0; i < m_InnerArray.Count; i++ )
            {
                if ( m_InnerArray[i] != a.m_InnerArray[i] )
                {
                    return BSObject.False;
                }
            }

            return BSObject.True;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }

}

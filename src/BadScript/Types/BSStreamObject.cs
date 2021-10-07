using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Types
{

    public class BSStreamObject : ABSObject
    {

        private readonly BSTable m_InstanceFunctions;
        private Stream m_Stream;

        #region Public

        public BSStreamObject( Stream fs ) : this( SourcePosition.Unknown, fs )
        {
        }

        public ABSTable GetInnerFunctionTable() => m_InstanceFunctions;

        public BSStreamObject( SourcePosition pos, Stream fs ) : base( pos )
        {
            m_Stream = fs;

            m_InstanceFunctions = new BSTable(
                                              SourcePosition.Unknown,
                                              new Dictionary < ABSObject, ABSObject >
                                              {
                                                  {
                                                      new BSObject( "close" ), new BSFunction(
                                                           "function close()",
                                                           args => CloseFileStream(),
                                                           0
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "write" ), new BSFunction(
                                                           "function write(str)",
                                                           WriteString,
                                                           1
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "writeb" ), new BSFunction(
                                                           "function writeb(str)",
                                                           WriteBinary,
                                                           1
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "writeLine" ), new BSFunction(
                                                           "function writeLine(str)",
                                                           WriteLine,
                                                           1
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "readLine" ), new BSFunction(
                                                           "function readLine()",
                                                           args => ReadLine(),
                                                           0
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "readbAll" ), new BSFunction(
                                                           "function readbAll()",
                                                           args => ReadAllBinary(),
                                                           0
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "readAll" ),
                                                      new BSFunction( "function readAll()", args => ReadAll(), 0 )
                                                  },
                                                  {
                                                      new BSObject( "getPosition" ), new BSFunction(
                                                           "function getPosition()",
                                                           args => GetPosition(),
                                                           0
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "setPosition" ), new BSFunction(
                                                           "function setPosition(pos)",
                                                           SetPosition,
                                                           1
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "getLength" ), new BSFunction(
                                                           "function getLength()",
                                                           args => GetLength(),
                                                           0
                                                          )
                                                  },
                                                  {
                                                      new BSObject( "setLength" ), new BSFunction(
                                                           "function setLength(len)",
                                                           SetLength,
                                                           1
                                                          )
                                                  }
                                              }
                                             );
        }

        public override bool Equals( ABSObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override ABSReference GetProperty( string propertyName )
        {
            return m_InstanceFunctions.GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_InstanceFunctions.HasProperty( propertyName );
        }

        public override ABSObject Invoke( ABSObject[] args )
        {
            throw new BSRuntimeException( Position, "File Stream API Objects can not be invoked" );
        }

        public override bool IsNull()
        {
            return false;
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return doneList[this] = m_Stream?.ToString() ?? "NULL(FileStream)";
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( Position, "File Stream API Objects can not be written to" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = m_Stream == null;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = decimal.Zero;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = $"Stream Object('{m_Stream}')";

            return false;
        }

        #endregion

        #region Protected

        protected override int GetHashCodeImpl()
        {
            return m_Stream.GetHashCode() ^ m_InstanceFunctions.GetHashCode();
        }

        #endregion

        #region Private

        private ABSObject CloseFileStream()
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            m_Stream.Close();
            m_Stream = null;

            return BSObject.Null;
        }

        private ABSObject GetLength()
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            return new BSObject( ( decimal )m_Stream.Length );
        }

        private ABSObject GetPosition()
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            return new BSObject( ( decimal )m_Stream.Position );
        }

        private ABSObject ReadAll()
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            using ( TextReader reader = new StreamReader( m_Stream ) )

            {
                return new BSObject( reader.ReadToEnd() );
            }
        }

        private ABSObject ReadAllBinary()
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            byte[] buf = new byte[m_Stream.Length];
            m_Stream.Read( buf, 0, buf.Length );
            BSArray a = new BSArray( buf.Select( x => new BSObject( ( decimal )x ) ) );

            return a;
        }

        private ABSObject ReadLine()
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            using ( TextReader reader = new StreamReader( m_Stream ) )

            {
                return new BSObject( reader.ReadLine() );
            }
        }

        private ABSObject SetLength( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();
            m_Stream.SetLength( ( long )o.ConvertDecimal() );

            return BSObject.Null;
        }

        private ABSObject SetPosition( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            m_Stream.Position = ( long )o.ConvertDecimal();

            return BSObject.Null;
        }

        private ABSObject WriteBinary( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            if ( o is ABSArray arr )
            {
                for ( int i = 0; i < arr.GetLength(); i++ )
                {
                    m_Stream.WriteByte( ( byte )arr.GetRawElement( i ).ConvertDecimal() );
                }
            }

            return BSObject.Null;
        }

        private ABSObject WriteLine( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            using ( TextWriter writer = new StreamWriter( m_Stream ) )
            {
                writer.WriteLine( o.ConvertString() );
            }

            return BSObject.Null;
        }

        private ABSObject WriteString( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( Position, "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            using ( TextWriter writer = new StreamWriter( m_Stream ) )
            {
                writer.Write( o.ConvertString() );
            }

            return BSObject.Null;
        }

        #endregion
        

    }

}

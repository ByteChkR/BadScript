using System.Collections.Generic;
using System.IO;
using BadScript.Common.Exceptions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.IO
{

    public class BSFileSystemObject : ABSObject
    {
        private BSTable m_InstanceFunctions;
        private FileStream m_Stream;
        private readonly string m_FilePath;

        public override bool IsNull => false;

        #region Public

        public BSFileSystemObject( string path, FileStream fs )
        {
            m_FilePath = path;
            m_Stream = fs;

            m_InstanceFunctions = new BSTable(
                new Dictionary < ABSObject, ABSObject >
                {
                    {
                        new BSObject( "close" ), new BSFunction(
                            "function close()",
                            CloseFileStream,
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
                        new BSObject( "writeLine" ), new BSFunction(
                            "function writeLine(str)",
                            WriteLine,
                            1
                        )
                    },
                    {
                        new BSObject( "readLine" ), new BSFunction(
                            "function readLine()",
                            ReadLine,
                            0
                        )
                    },
                    { new BSObject( "readAll" ), new BSFunction( "function readAll()", ReadAll, 0 ) },
                    {
                        new BSObject( "getPosition" ), new BSFunction(
                            "function getPosition()",
                            GetPosition,
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
                            GetLength,
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
            throw new BSRuntimeException( "File Stream API Objects can not be invoked" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return doneList[this] = m_Stream.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
        {
            throw new BSRuntimeException( "File Stream API Objects can not be written to" );
        }

        public override bool TryConvertBool( out bool v )
        {
            v = m_Stream == null;

            return true;
        }

        public override bool TryConvertDecimal( out decimal d )
        {
            d = 0;

            return false;
        }

        public override bool TryConvertString( out string v )
        {
            v = $"FileStream('{m_FilePath}')";

            return false;
        }

        #endregion

        #region Private

        private ABSObject CloseFileStream( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            m_Stream.Close();
            m_Stream = null;

            return new BSObject( 0 );
        }

        private ABSObject GetLength( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            return new BSObject( ( decimal ) m_Stream.Length );
        }

        private ABSObject GetPosition( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            return new BSObject( ( decimal ) m_Stream.Position );
        }

        private ABSObject ReadAll( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            using ( TextReader reader = new StreamReader( m_Stream ) )

            {
                return new BSObject( reader.ReadToEnd() );
            }
        }

        private ABSObject ReadLine( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
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
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();
            m_Stream.SetLength( ( long ) o.ConvertDecimal() );

            return new BSObject( 0 );

        }

        private ABSObject SetPosition( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            m_Stream.Position = ( long ) o.ConvertDecimal();

            return new BSObject( 0 );
        }

        private ABSObject WriteLine( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            using ( TextWriter writer = new StreamWriter( m_Stream ) )
            {
                writer.WriteLine( o.ConvertString() );
            }

            return new BSObject( 0 );
        }

        private ABSObject WriteString( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new BSRuntimeException( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            using ( TextWriter writer = new StreamWriter( m_Stream ) )
            {
                writer.Write( o.ConvertString() );
            }

            return new BSObject( 0 );
        }

        #endregion
    }

}

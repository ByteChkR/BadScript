using System;
using System.Collections.Generic;
using System.IO;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Apis.FileSystem
{

    public class BSFileSystemObject : BSRuntimeObject
    {

        private EngineRuntimeTable m_InstanceFunctions;
        private FileStream m_Stream;
        private readonly string m_FilePath;

        #region Public

        public BSFileSystemObject( string path, FileStream fs )
        {
            m_FilePath = path;
            m_Stream = fs;

            m_InstanceFunctions = new EngineRuntimeTable(
                                                         new Dictionary < BSRuntimeObject, BSRuntimeObject >
                                                         {
                                                             {
                                                                 new EngineRuntimeObject( "close" ),
                                                                 new BSRuntimeFunction(
                                                                      "function close()",
                                                                      CloseFileStream
                                                                     )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "write" ),
                                                                 new BSRuntimeFunction(
                                                                      "function write(str)",
                                                                      WriteString
                                                                     )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "writeLine" ),
                                                                 new BSRuntimeFunction(
                                                                      "function writeLine(str)",
                                                                      WriteLine
                                                                     )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "readLine" ),
                                                                 new BSRuntimeFunction(
                                                                      "function readLine()",
                                                                      ReadLine
                                                                     )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "readAll" ),
                                                                 new BSRuntimeFunction( "function readAll()", ReadAll )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "getPosition" ),
                                                                 new BSRuntimeFunction(
                                                                      "function getPosition()",
                                                                      GetPosition
                                                                     )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "setPosition" ),
                                                                 new BSRuntimeFunction(
                                                                      "function setPosition(pos)",
                                                                      SetPosition
                                                                     )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "getLength" ),
                                                                 new BSRuntimeFunction(
                                                                      "function getLength()",
                                                                      GetLength
                                                                     )
                                                             },
                                                             {
                                                                 new EngineRuntimeObject( "setLength" ),
                                                                 new BSRuntimeFunction(
                                                                      "function setLength(len)",
                                                                      SetLength
                                                                     )
                                                             }
                                                         }
                                                        );
        }

        public override bool Equals( BSRuntimeObject other )
        {
            return ReferenceEquals( this, other );
        }

        public override BSRuntimeReference GetProperty( string propertyName )
        {
            return m_InstanceFunctions.GetProperty( propertyName );
        }

        public override bool HasProperty( string propertyName )
        {
            return m_InstanceFunctions.HasProperty( propertyName );
        }

        public override BSRuntimeObject Invoke( BSRuntimeObject[] args )
        {
            throw new Exception( "File Stream API Objects can not be invoked" );
        }

        public override string SafeToString( Dictionary < BSRuntimeObject, string > doneList )
        {
            return doneList[this] = m_Stream.ToString();
        }

        public override void SetProperty( string propertyName, BSRuntimeObject obj )
        {
            throw new Exception( "File Stream API Objects can not be written to" );
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

        private BSRuntimeObject CloseFileStream( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            m_Stream.Close();
            m_Stream = null;

            return new EngineRuntimeObject( 0 );
        }

        private BSRuntimeObject GetLength( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            return new EngineRuntimeObject( ( decimal ) m_Stream.Length );
        }

        private BSRuntimeObject GetPosition( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            return new EngineRuntimeObject( ( decimal ) m_Stream.Position );
        }

        private BSRuntimeObject ReadAll( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            TextReader reader = new StreamReader( m_Stream );

            return new EngineRuntimeObject( reader.ReadToEnd() );
        }

        private BSRuntimeObject ReadLine( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            TextReader reader = new StreamReader( m_Stream );

            return new EngineRuntimeObject( reader.ReadLine() );
        }

        private BSRuntimeObject SetLength( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            BSRuntimeObject o = arg[0];

            if ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o.TryConvertDecimal( out decimal path ) )
            {
                m_Stream.SetLength( ( long ) path );

                return new EngineRuntimeObject( 0 );
            }

            throw new Exception( "Expected Decimal" );
        }

        private BSRuntimeObject SetPosition( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            BSRuntimeObject o = arg[0];

            if ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o.TryConvertDecimal( out decimal path ) )
            {
                m_Stream.Position = ( long ) path;

                return new EngineRuntimeObject( 0 );
            }

            throw new Exception( "Expected Decimal" );
        }

        private BSRuntimeObject WriteLine( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            BSRuntimeObject o = arg[0];

            if ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o.TryConvertString( out string path ) )
            {
                TextWriter writer = new StreamWriter( m_Stream );
                writer.WriteLine( path );

                return new EngineRuntimeObject( 0 );
            }

            throw new Exception( "Expected String" );
        }

        private BSRuntimeObject WriteString( BSRuntimeObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            BSRuntimeObject o = arg[0];

            if ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o.TryConvertString( out string path ) )
            {
                TextWriter writer = new StreamWriter( m_Stream );
                writer.Write( path );

                return new EngineRuntimeObject( 0 );
            }

            throw new Exception( "Expected String" );
        }

        #endregion

    }

}

﻿using System;
using System.Collections.Generic;
using System.IO;

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

        #region Public

        public BSFileSystemObject( string path, FileStream fs )
        {
            m_FilePath = path;
            m_Stream = fs;

            m_InstanceFunctions = new BSTable(
                                                         new Dictionary < ABSObject, ABSObject >
                                                         {
                                                             {
                                                                 new BSObject( "close" ),
                                                                 new BSFunction(
                                                                      "function close()",
                                                                      CloseFileStream
                                                                     )
                                                             },
                                                             {
                                                                 new BSObject( "write" ),
                                                                 new BSFunction(
                                                                      "function write(str)",
                                                                      WriteString
                                                                     )
                                                             },
                                                             {
                                                                 new BSObject( "writeLine" ),
                                                                 new BSFunction(
                                                                      "function writeLine(str)",
                                                                      WriteLine
                                                                     )
                                                             },
                                                             {
                                                                 new BSObject( "readLine" ),
                                                                 new BSFunction(
                                                                      "function readLine()",
                                                                      ReadLine
                                                                     )
                                                             },
                                                             {
                                                                 new BSObject( "readAll" ),
                                                                 new BSFunction( "function readAll()", ReadAll )
                                                             },
                                                             {
                                                                 new BSObject( "getPosition" ),
                                                                 new BSFunction(
                                                                      "function getPosition()",
                                                                      GetPosition
                                                                     )
                                                             },
                                                             {
                                                                 new BSObject( "setPosition" ),
                                                                 new BSFunction(
                                                                      "function setPosition(pos)",
                                                                      SetPosition
                                                                     )
                                                             },
                                                             {
                                                                 new BSObject( "getLength" ),
                                                                 new BSFunction(
                                                                      "function getLength()",
                                                                      GetLength
                                                                     )
                                                             },
                                                             {
                                                                 new BSObject( "setLength" ),
                                                                 new BSFunction(
                                                                      "function setLength(len)",
                                                                      SetLength
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
            throw new Exception( "File Stream API Objects can not be invoked" );
        }

        public override string SafeToString( Dictionary < ABSObject, string > doneList )
        {
            return doneList[this] = m_Stream.ToString();
        }

        public override void SetProperty( string propertyName, ABSObject obj )
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

        private ABSObject CloseFileStream( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            m_Stream.Close();
            m_Stream = null;

            return new BSObject( 0 );
        }

        private ABSObject GetLength( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            return new BSObject( ( decimal ) m_Stream.Length );
        }

        private ABSObject GetPosition( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            return new BSObject( ( decimal ) m_Stream.Position );
        }

        private ABSObject ReadAll( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            TextReader reader = new StreamReader( m_Stream );

            return new BSObject( reader.ReadToEnd() );
        }

        private ABSObject ReadLine( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            TextReader reader = new StreamReader( m_Stream );

            return new BSObject( reader.ReadLine() );
        }

        private ABSObject SetLength( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertDecimal( out decimal path ) )
            {
                m_Stream.SetLength( ( long ) path );

                return new BSObject( 0 );
            }

            throw new Exception( "Expected Decimal" );
        }

        private ABSObject SetPosition( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertDecimal( out decimal path ) )
            {
                m_Stream.Position = ( long ) path;

                return new BSObject( 0 );
            }

            throw new Exception( "Expected Decimal" );
        }

        private ABSObject WriteLine( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                TextWriter writer = new StreamWriter( m_Stream );
                writer.WriteLine( path );

                return new BSObject( 0 );
            }

            throw new Exception( "Expected String" );
        }

        private ABSObject WriteString( ABSObject[] arg )
        {
            if ( m_Stream == null )
            {
                throw new Exception( "File Stream is Disposed" );
            }

            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                TextWriter writer = new StreamWriter( m_Stream );
                writer.Write( path );

                return new BSObject( 0 );
            }

            throw new Exception( "Expected String" );
        }

        #endregion

    }

}

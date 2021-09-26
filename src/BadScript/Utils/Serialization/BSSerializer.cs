using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Utils.Serialization.Serializers;

namespace BadScript.Utils.Serialization
{

    public static class BSSerializer
    {

        private static readonly List < BSExpressionSerializer > s_Compilers = new List < BSExpressionSerializer >
                                                                              {
                                                                                  new
                                                                                      BsArrayAccessExpressionSerializer(),
                                                                                  new BsArrayExpressionSerializer(),
                                                                                  new BsBreakExpressionSerializer(),
                                                                                  new BsContinueExpressionSerializer(),
                                                                                  new BsInvocationExpressionSerializer(),
                                                                                  new BsPropertyExpressionSerializer(),
                                                                                  new BsReturnExpressionSerializer(),
                                                                                  new BsTableExpressionSerializer(),
                                                                                  new BsValueExpressionSerializer(),
                                                                                  new
                                                                                      BsNullCheckPropertyExpressionSerializer(),
                                                                                  new BsAssignExpressionSerializer(),
                                                                                  new BsWhileExpressionSerializer(),
                                                                                  new BsBlockExpressionSerializer(),
                                                                                  new BsTryExpressionSerializer(),
                                                                                  new BsIfExpressionSerializer(),
                                                                                  new
                                                                                      BsEnumerableFunctionDefinitionExpressionSerializer(),
                                                                                  new BsForeachExpressionSerializer(),
                                                                                  new BsForExpressionSerializer(),
                                                                                  new
                                                                                      BsFunctionDefinitionExpressionSerializer(),
                                                                                  new BsProxyExpressionSerializer()
                                                                              };

        #region Public

        public static BSExpression[] Deserialize( Stream s )
        {
            BSSerializedHeader header = ReadHeader( s );

            if ( ( header.SerializerHints & BSSerializerHints.Compressed ) != 0 )
            {
                s = new GZipStream( s, CompressionMode.Decompress );
            }

            return s.DeserializeBlock();
        }

        public static void Serialize( BSExpression[] src, Stream s, BSSerializerHints hints )
        {
            WriteHeader( s, hints );

            if ( ( hints & BSSerializerHints.Compressed ) != 0 )
            {
                s = new GZipStream( s, CompressionLevel.Optimal );
            }

            s.SerializeBlock( src );
        }

        public static void Serialize( BSExpression[] src, Stream s )
        {
            Serialize( src, s, BSSerializerHints.Default );
        }

        //public static byte[] Serialize( BSExpression[] src )
        //{
        //    MemoryStream ms = new MemoryStream();
        //    Serialize( src, ms );
        //    return ms.ToArray();
        //}

        internal static BSExpression[] DeserializeBlock( this Stream s )
        {
            int blockSize = s.DeserializeInt32();
            List < BSExpression > exprs = new List < BSExpression >();

            for ( int i = 0; i < blockSize; i++ )
            {
                exprs.Add( DeserializeExpression( s ) );
            }

            return exprs.ToArray();
        }

        internal static bool DeserializeBool( this Stream s )
        {
            byte[] cBuf = new byte[sizeof( bool )];
            s.Read( cBuf, 0, cBuf.Length );

            return BitConverter.ToBoolean( cBuf, 0 );
        }

        internal static decimal DeserializeDecimal( this Stream s )
        {
            byte[] b = new byte[sizeof( double )];
            s.Read( b, 0, b.Length );

            return ( decimal )BitConverter.ToDouble( b, 0 );
        }

        internal static BSExpression DeserializeExpression( this Stream s )
        {
            List < byte > ret = new List < byte >();

            BSCompiledExpressionCode code = s.DeserializeOpCode();
            BSExpressionSerializer c = s_Compilers.First( x => x.CanDeserialize( code ) );

            return c.Deserialize( code, s );
        }

        internal static BSFunctionParameter[] DeserializeFunctionParameters( this Stream s )
        {
            int c = s.DeserializeInt32();
            BSFunctionParameter[] ret = new BSFunctionParameter[c];

            for ( int i = 0; i < c; i++ )
            {
                ret[i] = new BSFunctionParameter(
                                                 s.DeserializeString(),
                                                 s.DeserializeBool(),
                                                 s.DeserializeBool(),
                                                 s.DeserializeBool()
                                                );
            }

            return ret;
        }

        internal static int DeserializeInt32( this Stream s )
        {
            byte[] cBuf = new byte[sizeof( int )];
            s.Read( cBuf, 0, cBuf.Length );

            return BitConverter.ToInt32( cBuf, 0 );
        }

        internal static Dictionary < BSExpression, BSExpression[] > DeserializeMap( this Stream s )
        {
            int c = s.DeserializeInt32();

            Dictionary < BSExpression, BSExpression[] > map = new Dictionary < BSExpression, BSExpression[] >();

            for ( int i = 0; i < c; i++ )
            {
                BSExpression k = s.DeserializeExpression();
                map[k] = s.DeserializeBlock();
            }

            return map;
        }

        internal static Dictionary < string, BSExpression > DeserializeNameMap( this Stream s )
        {
            int c = s.DeserializeInt32();

            Dictionary < string, BSExpression > map = new Dictionary < string, BSExpression >();

            for ( int i = 0; i < c; i++ )
            {
                string k = s.DeserializeString();
                map[k] = s.DeserializeExpression();
            }

            return map;
        }

        internal static BSCompiledExpressionCode DeserializeOpCode( this Stream s )
        {
            return ( BSCompiledExpressionCode )s.ReadByte();
        }

        internal static BSSerializerHints DeserializeSHint( this Stream s )
        {
            return ( BSSerializerHints )s.ReadByte();
        }

        internal static string DeserializeString( this Stream s )
        {
            int c = s.DeserializeInt32();
            byte[] sBuf = new byte[c];
            s.Read( sBuf, 0, sBuf.Length );

            return Encoding.UTF8.GetString( sBuf );
        }

        internal static void SerializeBlock( this Stream l, BSExpression[] src )
        {
            l.SerializeInt32( src.Length );

            foreach ( BSExpression bsExpression in src )
            {
                l.SerializeExpression( bsExpression );
            }
        }

        internal static void SerializeBool( this Stream l, bool b )
        {
            l.Write( BitConverter.GetBytes( b ), 0, sizeof( bool ) );
        }

        internal static void SerializeDecimal( this Stream l, decimal n )
        {
            byte[] b = BitConverter.GetBytes( ( double )n );
            l.Write( b, 0, b.Length );
        }

        internal static void SerializeExpression( this Stream l, BSExpression expr )
        {
            BSExpressionSerializer c = s_Compilers.First( x => x.CanSerialize( expr ) );
            c.Serialize( expr, l );
        }

        internal static void SerializeFunctionParameters( this Stream l, BSFunctionParameter[] args )
        {
            l.SerializeInt32( args.Length );

            foreach ( BSFunctionParameter bsFunctionParameter in args )
            {
                l.SerializeString( bsFunctionParameter.Name );
                l.SerializeBool( bsFunctionParameter.NotNull );
                l.SerializeBool( bsFunctionParameter.IsOptional );
                l.SerializeBool( bsFunctionParameter.IsArgArray );
            }
        }

        internal static void SerializeInt32( this Stream l, int n )
        {
            byte[] b = BitConverter.GetBytes( n );
            l.Write( b, 0, b.Length );
        }

        internal static void SerializeMap( this Stream l, Dictionary < BSExpression, BSExpression[] > map )
        {
            l.SerializeInt32( map.Count );

            foreach ( KeyValuePair < BSExpression, BSExpression[] > keyValuePair in map )
            {
                l.SerializeExpression( keyValuePair.Key );
                l.SerializeBlock( keyValuePair.Value );
            }
        }

        internal static void SerializeNameMap( this Stream l, Dictionary < string, BSExpression > map )
        {
            l.SerializeInt32( map.Count );

            foreach ( KeyValuePair < string, BSExpression > keyValuePair in map )
            {
                l.SerializeString( keyValuePair.Key );
                l.SerializeExpression( keyValuePair.Value );
            }
        }

        internal static void SerializeOpCode( this Stream l, BSCompiledExpressionCode code )
        {
            l.Write( new[] { ( byte )code }, 0, 1 );
        }

        internal static void SerializeSHint( this Stream l, BSSerializerHints hint )
        {
            l.Write( new[] { ( byte )hint }, 0, 1 );
        }

        internal static void SerializeString( this Stream l, string str )
        {
            byte[] b = Encoding.UTF8.GetBytes( str );
            byte[] bl = BitConverter.GetBytes( b.Length );
            l.Write( bl, 0, bl.Length );
            l.Write( b, 0, b.Length );
        }

        #endregion

        #region Private

        private static BSSerializedHeader ReadHeader( Stream s )
        {
            BSSerializedHeader header = new BSSerializedHeader();
            header.Magic = new byte[4];
            s.Read( header.Magic, 0, 4 );

            header.SerializerHints = s.DeserializeSHint();
            header.BSSerializerFormatVersion = s.DeserializeString();

            if ( !header.IsValidHeader )
            {
                throw new BSRuntimeException( "Invalid Header for Serialized BS Expressions" );
            }

            return header;
        }

        private static void WriteHeader( Stream s, BSSerializerHints hints )
        {
            BSSerializedHeader header = BSSerializedHeader.CreateEmpty( hints );

            s.Write( header.Magic, 0, header.Magic.Length );
            s.SerializeSHint( header.SerializerHints );
            s.SerializeString( header.BSSerializerFormatVersion );
        }

        #endregion

    }

}

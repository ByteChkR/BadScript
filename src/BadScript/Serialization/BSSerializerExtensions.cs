using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Serialization.Serializers;

namespace BadScript.Serialization
{

    public static class BSSerializerExtensions
    {

        #region Public

        public static BSExpression[] DeserializeBlock( this Stream s, BSSerializerContext context )
        {
            int blockSize = s.DeserializeInt32();
            List < BSExpression > exprs = new List < BSExpression >();

            for ( int i = 0; i < blockSize; i++ )
            {
                exprs.Add( DeserializeExpression( s, context ) );
            }

            return exprs.ToArray();
        }

        public static bool DeserializeBool( this Stream s )
        {
            byte[] cBuf = new byte[sizeof( bool )];
            s.Read( cBuf, 0, cBuf.Length );

            return BitConverter.ToBoolean( cBuf, 0 );
        }

        public static decimal DeserializeDecimal( this Stream s )
        {
            byte[] b = new byte[sizeof( double )];
            s.Read( b, 0, b.Length );

            return ( decimal )BitConverter.ToDouble( b, 0 );
        }

        public static BSExpression DeserializeExpression( this Stream s, BSSerializerContext context )
        {
            BSCompiledExpressionCode code = s.DeserializeOpCode();
            BSExpressionSerializer c = BSSerializer.Compilers.FirstOrDefault( x => x.CanDeserialize( code ) );

            if ( c == null )
            {
                throw new BSSerializerException( $"Can not find serializer for Expression Code '{code}'" );
            }

            return c.Deserialize( code, s, context );
        }

        public static BSFunctionParameter[] DeserializeFunctionParameters(
            this Stream s,
            BSSerializerContext context )
        {
            int c = s.DeserializeInt32();
            BSFunctionParameter[] ret = new BSFunctionParameter[c];

            for ( int i = 0; i < c; i++ )
            {
                ret[i] = new BSFunctionParameter(
                                                 s.DeserializeString( context ),
                                                 s.DeserializeBool(),
                                                 s.DeserializeBool(),
                                                 s.DeserializeBool()
                                                );
            }

            return ret;
        }

        public static int DeserializeInt32( this Stream s )
        {
            byte[] cBuf = new byte[sizeof( int )];
            s.Read( cBuf, 0, cBuf.Length );

            return BitConverter.ToInt32( cBuf, 0 );
        }

        public static Dictionary < BSExpression, BSExpression[] > DeserializeMap(
            this Stream s,
            BSSerializerContext context )
        {
            int c = s.DeserializeInt32();

            Dictionary < BSExpression, BSExpression[] > map = new Dictionary < BSExpression, BSExpression[] >();

            for ( int i = 0; i < c; i++ )
            {
                BSExpression k = s.DeserializeExpression( context );
                map[k] = s.DeserializeBlock( context );
            }

            return map;
        }

        public static Dictionary < string, BSExpression > DeserializeNameMap(
            this Stream s,
            BSSerializerContext context )
        {
            int c = s.DeserializeInt32();

            Dictionary < string, BSExpression > map = new Dictionary < string, BSExpression >();

            for ( int i = 0; i < c; i++ )
            {
                string k = s.DeserializeString( context );
                map[k] = s.DeserializeExpression( context );
            }

            return map;
        }

        public static BSCompiledExpressionCode DeserializeOpCode( this Stream s )
        {
            return ( BSCompiledExpressionCode )s.ReadByte();
        }

        public static BSSerializerHints DeserializeSHint( this Stream s )
        {
            return ( BSSerializerHints )s.ReadByte();
        }

        public static string DeserializeString( this Stream s, BSSerializerContext context )
        {
            if ( context != null )
            {
                return context.ResolveCached( s.DeserializeInt32() );
            }

            int c = s.DeserializeInt32();
            byte[] sBuf = new byte[c];
            s.Read( sBuf, 0, sBuf.Length );

            return Encoding.UTF8.GetString( sBuf );
        }

        public static string[] DeserializeStringArray( this Stream s, BSSerializerContext context )
        {
            int count = s.DeserializeInt32();
            string[] arr = new string[count];

            for ( int i = 0; i < count; i++ )
            {
                arr[i] = s.DeserializeString( context );
            }

            return arr;
        }

        public static void SerializeBlock( this Stream l, BSExpression[] src, BSSerializerContext context )
        {
            l.SerializeInt32( src.Length );

            foreach ( BSExpression bsExpression in src )
            {
                l.SerializeExpression( bsExpression, context );
            }
        }

        public static void SerializeBool( this Stream l, bool b )
        {
            l.Write( BitConverter.GetBytes( b ), 0, sizeof( bool ) );
        }

        public static void SerializeDecimal( this Stream l, decimal n )
        {
            byte[] b = BitConverter.GetBytes( ( double )n );
            l.Write( b, 0, b.Length );
        }

        public static void SerializeExpression( this Stream l, BSExpression expr, BSSerializerContext context )
        {
            BSExpressionSerializer c = BSSerializer.Compilers.FirstOrDefault( x => x.CanSerialize( expr ) );

            if ( c == null )
            {
                throw new BSSerializerException( $"Can not find serializer for Expression '{expr.GetType()}'" );
            }

            c.Serialize( expr, l, context );
        }

        public static void SerializeFunctionParameters(
            this Stream l,
            BSFunctionParameter[] args,
            BSSerializerContext context )
        {
            l.SerializeInt32( args.Length );

            foreach ( BSFunctionParameter bsFunctionParameter in args )
            {
                l.SerializeString( bsFunctionParameter.Name, context );
                l.SerializeBool( bsFunctionParameter.NotNull );
                l.SerializeBool( bsFunctionParameter.IsOptional );
                l.SerializeBool( bsFunctionParameter.IsArgArray );
            }
        }

        public static void SerializeInt32( this Stream l, int n )
        {
            byte[] b = BitConverter.GetBytes( n );
            l.Write( b, 0, b.Length );
        }

        public static void SerializeMap(
            this Stream l,
            Dictionary < BSExpression, BSExpression[] > map,
            BSSerializerContext context )
        {
            l.SerializeInt32( map.Count );

            foreach ( KeyValuePair < BSExpression, BSExpression[] > keyValuePair in map )
            {
                l.SerializeExpression( keyValuePair.Key, context );
                l.SerializeBlock( keyValuePair.Value, context );
            }
        }

        public static void SerializeNameMap(
            this Stream l,
            Dictionary < string, BSExpression > map,
            BSSerializerContext context )
        {
            l.SerializeInt32( map.Count );

            foreach ( KeyValuePair < string, BSExpression > keyValuePair in map )
            {
                l.SerializeString( keyValuePair.Key, context );
                l.SerializeExpression( keyValuePair.Value, context );
            }
        }

        public static void SerializeOpCode( this Stream l, BSCompiledExpressionCode code )
        {
            l.Write( new[] { ( byte )code }, 0, 1 );
        }

        public static void SerializeSHint( this Stream l, BSSerializerHints hint )
        {
            l.Write( new[] { ( byte )hint }, 0, 1 );
        }

        public static void SerializeString( this Stream l, string str, BSSerializerContext context )
        {
            if ( context != null )
            {
                l.SerializeInt32( context.AddCached( str ) );

                return;
            }

            byte[] b = Encoding.UTF8.GetBytes( str );
            byte[] bl = BitConverter.GetBytes( b.Length );
            l.Write( bl, 0, bl.Length );
            l.Write( b, 0, b.Length );
        }

        public static void SerializeStringArray( this Stream s, string[] arr, BSSerializerContext context )
        {
            s.SerializeInt32( arr.Length );

            foreach ( string s1 in arr )
            {
                s.SerializeString( s1, context );
            }
        }

        #endregion

    }

}

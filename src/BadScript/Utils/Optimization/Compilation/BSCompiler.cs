using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;

namespace BadScript.Utils.Optimization.Compilation
{

    public static class BSCompiler
    {
        private static readonly List<BSExpressionCompiler> s_Compilers = new List<BSExpressionCompiler>
        {
            new BSArrayAccessExpressionCompiler(),
            new BSArrayExpressionCompiler(),
            new BSBreakExpressionCompiler(),
            new BSContinueExpressionCompiler(),
            new BSInvocationExpressionCompiler(),
            new BSPropertyExpressionCompiler(),
            new BSReturnExpressionCompiler(),
            new BSTableExpressionCompiler(),
            new BSValueExpressionCompiler(),
            new BSNullCheckPropertyExpressionCompiler(),
            new BSAssignExpressionCompiler(),
            new BSWhileExpressionCompiler(),
            new BSTryExpressionCompiler(),
            new BSIfExpressionCompiler(),
            new BSEnumerableFunctionDefinitionExpressionCompiler(),
            new BSForeachExpressionCompiler(),
            new BSForExpressionCompiler(),
            new BSFunctionDefinitionExpressionCompiler(),
            new BSProxyExpressionCompiler()
        };

        internal static void SerializeOpCode(this List<byte> l, BSCompiledExpressionCode code)
        {
            l.Add((byte)code);
        }

        internal static void SerializeMap(this List<byte> l, Dictionary<BSExpression, BSExpression[]> map)
        {
            l.SerializeInt32(map.Count);

            foreach (KeyValuePair<BSExpression, BSExpression[]> keyValuePair in map)
            {
                l.SerializeExpression(keyValuePair.Key);
                l.SerializeBlock(keyValuePair.Value);
            }
        }

        internal static Dictionary<BSExpression, BSExpression[]> DeserializeMap(this Stream s)
        {
            int c = s.DeserializeInt32();

            Dictionary<BSExpression, BSExpression[]> map = new Dictionary<BSExpression, BSExpression[]>();
            for (int i = 0; i < c; i++)
            {
                BSExpression k = s.DeserializeExpression();
                map[k] = s.DeserializeBlock();
            }

            return map;
        }

        internal static void SerializeNameMap(this List<byte> l, Dictionary<string, BSExpression> map)
        {
            l.SerializeInt32(map.Count);

            foreach (KeyValuePair<string, BSExpression> keyValuePair in map)
            {
                l.SerializeString(keyValuePair.Key);
                l.SerializeExpression(keyValuePair.Value);
            }
        }

        internal static Dictionary<string, BSExpression> DeserializeNameMap(this Stream s)
        {
            int c = s.DeserializeInt32();

            Dictionary<string, BSExpression> map = new Dictionary<string, BSExpression>();
            for (int i = 0; i < c; i++)
            {
                string k = s.DeserializeString();
                map[k] = s.DeserializeExpression();
            }

            return map;
        }

        internal static BSCompiledExpressionCode DeserializeOpCode(this Stream s)
        {
            return ( BSCompiledExpressionCode ) s.ReadByte();
        }

        internal static void SerializeDecimal( this List < byte > l, decimal n )
        {
            l.AddRange( BitConverter.GetBytes( ( double ) n ) );
        }
        internal static decimal DeserializeDecimal(this Stream s)
        {
            byte[] b = new byte[sizeof( double )];
            s.Read( b, 0, b.Length );

            return ( decimal ) BitConverter.ToDouble( b ,0);
        }

        internal static void SerializeInt32( this List < byte > l, int n )
        {
            l.AddRange( BitConverter.GetBytes( n ) );
        }
        internal static int DeserializeInt32(this Stream s)
        {
            byte[] cBuf = new byte[sizeof(int)];
            s.Read(cBuf, 0, cBuf.Length);
            return BitConverter.ToInt32(cBuf, 0);
        }

        internal static void SerializeBool( this List < byte > l, bool b )
        {
            l.AddRange( BitConverter.GetBytes( b ) );
        }

        internal static bool DeserializeBool(this Stream s)
        {
            byte[] cBuf = new byte[sizeof(bool)];
            s.Read(cBuf, 0, cBuf.Length);
            return BitConverter.ToBoolean(cBuf, 0);
        }

        internal static string DeserializeString(this Stream s)
        {
            int c = s.DeserializeInt32();
            byte[] sBuf = new byte[c];
            s.Read( sBuf, 0, sBuf.Length );

            return Encoding.UTF8.GetString( sBuf );
        }

        internal static void SerializeFunctionParameters(this List <byte> l, BSFunctionParameter[] args)
        {
            l.SerializeInt32( args.Length );
            foreach (BSFunctionParameter bsFunctionParameter in args)
            {
                l.SerializeString(bsFunctionParameter.Name);
                l.SerializeBool(bsFunctionParameter.NotNull);
                l.SerializeBool(bsFunctionParameter.IsOptional);
            }
        }

        internal static BSFunctionParameter[] DeserializeFunctionParameters(this Stream s)
        {
            int c = s.DeserializeInt32();
            BSFunctionParameter[] ret = new BSFunctionParameter[c];

            for ( int i = 0; i < c; i++ )
            {
                ret[i] = new BSFunctionParameter( s.DeserializeString(), s.DeserializeBool(), s.DeserializeBool() );
            }

            return ret;
        }

        internal static void SerializeString(this List<byte> l, string str)
        {
            byte[] b = Encoding.UTF8.GetBytes(str);
            l.AddRange(BitConverter.GetBytes(b.Length));
            l.AddRange(b);
        }

        internal static BSExpression DeserializeExpression(this Stream s)
        {
            List<byte> ret = new List<byte>();

            BSCompiledExpressionCode code = s.DeserializeOpCode();
            BSExpressionCompiler c = s_Compilers.First(x => x.CanDeserialize(code));

            return c.Deserialize(code, s);
        }

        internal static void SerializeExpression(this List <byte> l, BSExpression expr)
        {
            l.AddRange( expr.SerializeExpression( ) );
        }
        internal static byte[] SerializeExpression(this BSExpression expr)
        {
            BSExpressionCompiler c = s_Compilers.First(x => x.CanSerialize(expr));

            return c.Serialize(expr);
        }



        internal static BSExpression[] DeserializeBlock(this Stream s)
        {
            int blockSize = s.DeserializeInt32();
            List<BSExpression> exprs = new List<BSExpression>();
            for (int i = 0; i < blockSize; i++)
            {
                exprs.Add(DeserializeExpression(s));
            }

            return exprs.ToArray();
        }


        internal static void SerializeBlock( this List < byte > l, BSExpression[] src )
        {
            l.SerializeInt32(src.Length);
            foreach (BSExpression bsExpression in src)
            {
                l.SerializeExpression(bsExpression);
            }
        }

        public static byte[] Serialize(BSExpression[] src)
        {

            List<byte> ret = new List<byte>();
            ret.SerializeBlock( src );
            return ret.ToArray();
        }

        public static BSExpression[] Deserialize(Stream s)
        {
            return s.DeserializeBlock();
        }

    }

}
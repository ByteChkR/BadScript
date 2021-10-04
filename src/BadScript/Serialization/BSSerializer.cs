using System.Collections.Generic;
using System.IO;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Serialization.Serializers;

namespace BadScript.Serialization
{

    public static class BSSerializer
    {

        public static void AddSerializer( BSExpressionSerializer serializer )
        {
            s_Compilers.Add( serializer );
        }
        public static IEnumerable < BSExpressionSerializer > Compilers => s_Compilers;
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
                                                                                  new BsProxyExpressionSerializer(),
                                                                                  new BsNewClassExpressionSerializer(),
                                                                                  new BsClassDefExpressionSerializer(),
                                                                                  new BsUsingExpressionSerializer(),
                                                                                  new BsNamespaceExpressionSerializer()
                                                                              };

        #region Public

        public static BSExpression[] Deserialize( Stream s )
        {
            BSSerializedHeader header = ReadHeader( s );

            if ( ( header.SerializerHints & BSSerializerHints.NoStringCache ) == 0 )
            {
                long codeSectionEnd = s.DeserializeInt32();
                long codeStart = s.Position;

                s.Position = codeSectionEnd;

                BSSerializerContext context = BSSerializerContext.Deserialize( s );

                if ( s.Length != s.Position )
                {
                    throw new BSSerializerException( "Context is not located at the end of the file. Invalid File!" );
                }

                s.Position = codeStart;

                BSExpression[] exprs = s.DeserializeBlock( context );

                if ( codeSectionEnd != s.Position )
                {
                    throw new BSSerializerException(
                                                    "Code section is not directly followed by Context. Invalid File!"
                                                   );
                }

                return exprs;
            }

            return s.DeserializeBlock( null );
        }

        public static void Serialize( BSExpression[] src, Stream s, BSSerializerHints hints )
        {
            WriteHeader( s, hints );

            BSSerializerContext context = null;

            if ( ( hints & BSSerializerHints.NoStringCache ) == 0 )
            {
                context = new BSSerializerContext();
            }

            long codeSectionStart = s.Position;

            if ( ( hints & BSSerializerHints.NoStringCache ) == 0 )
            {
                s.SerializeInt32( 0 ); //Reserve Position for codeSectionEnd
            }

            s.SerializeBlock( src, context );

            if ( ( hints & BSSerializerHints.NoStringCache ) == 0 )
            {
                long codeSectionEnd = s.Position;
                s.Position = codeSectionStart;
                s.SerializeInt32( ( int )codeSectionEnd );
                s.Position = codeSectionEnd;
                context?.Serialize( s );
            }
        }

        public static void Serialize( BSExpression[] src, Stream s )
        {
            Serialize( src, s, BSSerializerHints.Default );
        }

        

        #endregion

        #region Private

        private static BSSerializedHeader ReadHeader( Stream s )
        {
            BSSerializedHeader header = new BSSerializedHeader();
            header.Magic = new byte[4];
            s.Read( header.Magic, 0, 4 );

            header.SerializerHints = s.DeserializeSHint();
            header.BSSerializerFormatVersion = s.DeserializeString( null );

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
            s.SerializeString( header.BSSerializerFormatVersion, null );
        }

        #endregion

    }

}

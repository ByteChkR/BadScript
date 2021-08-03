using System.Collections.Generic;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BadScript.Json
{

    internal static class Json2BS
    {
        #region Public

        public static ABSObject Convert( ABSObject[] args )
        {
            ABSObject o = args[0].ResolveReference();

            JToken jsonO = ( JToken ) JsonConvert.DeserializeObject( o.ConvertString() );

            ABSObject ret = Convert( jsonO );

            return ret;

        }

        #endregion

        #region Private

        private static ABSObject Convert( JToken jsonT )
        {
            if ( jsonT is JArray a )
            {
                return Convert( a );
            }

            if ( jsonT is JObject o )
            {
                return Convert( o );
            }

            if ( jsonT.Type == JTokenType.Boolean )
            {
                return new BSObject( jsonT.Value < bool >() ? 1 : 0 );
            }

            if ( jsonT.Type == JTokenType.Float || jsonT.Type == JTokenType.Integer )
            {
                return new BSObject( jsonT.Value < decimal >() );
            }

            return ConvertValue( jsonT );
        }

        private static ABSArray Convert( JArray jsonA )
        {
            BSArray a = new BSArray();

            for ( int i = 0; i < jsonA.Count; i++ )
            {
                JToken jToken = jsonA[i];
                a.InsertElement( i, Convert( jToken ) );
            }

            return a;
        }

        private static ABSObject Convert( JObject jsonO )
        {
            BSTable t = new BSTable( SourcePosition.Unknown );

            foreach ( KeyValuePair < string, JToken > kvp in jsonO )
            {
                t.InsertElement( new BSObject( kvp.Key ), Convert( kvp.Value ) );
            }

            return t;
        }

        private static ABSObject ConvertValue( JToken token )
        {
            switch ( token.Type )
            {
                case JTokenType.String:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.Date:
                    return new BSObject( token.Value < string >() );

                case JTokenType.Integer:
                    return new BSObject( ( decimal ) token.Value < int >() );

                case JTokenType.Float:
                    return new BSObject( ( decimal ) token.Value < float >() );

                case JTokenType.Boolean:
                    return new BSObject( token.Value < bool >() ? 1 : 0 );

                case JTokenType.Null:
                    return BSObject.Null;

                default:
                    throw new JsonConverterException( $"Expected Compatible Json Type. Got {token.Type}" );
            }
        }

        #endregion
    }

}

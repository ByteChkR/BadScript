using System;
using System.Collections.Generic;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BadScript.Json
{

    internal static class Json2BS
    {

        #region Public

        public static BSRuntimeObject Convert( BSRuntimeObject[] args )
        {
            BSRuntimeObject o = args[0];

            if ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o.TryConvertString( out string path ) )
            {
                JToken jsonO = ( JToken ) JsonConvert.DeserializeObject( path );

                BSRuntimeObject ret = Convert( jsonO );

                return ret;
            }

            throw new Exception( "Expected String" );
        }

        #endregion

        #region Private

        private static BSRuntimeObject Convert( JToken jsonT )
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
                return new EngineRuntimeObject( jsonT.Value < bool >() ? 1 : 0 );
            }

            if ( jsonT.Type == JTokenType.Float || jsonT.Type == JTokenType.Integer )
            {
                return new EngineRuntimeObject( jsonT.Value < decimal >() );
            }

            return ConvertValue( jsonT );
        }

        private static BSRuntimeArray Convert( JArray jsonA )
        {
            EngineRuntimeArray a = new EngineRuntimeArray();

            for ( int i = 0; i < jsonA.Count; i++ )
            {
                JToken jToken = jsonA[i];
                a.InsertElement( i, Convert( jToken ) );
            }

            return a;
        }

        private static BSRuntimeObject Convert( JObject jsonO )
        {
            EngineRuntimeTable t = new EngineRuntimeTable();

            foreach ( KeyValuePair < string, JToken > kvp in jsonO )
            {
                t.InsertElement( new EngineRuntimeObject( kvp.Key ), Convert( kvp.Value ) );
            }

            return t;
        }

        private static BSRuntimeObject ConvertValue( JToken token )
        {
            switch ( token.Type )
            {
                case JTokenType.String:
                case JTokenType.Guid:
                case JTokenType.Uri:
                    return new EngineRuntimeObject( token.Value < string >() );

                case JTokenType.Integer:
                    return new EngineRuntimeObject( ( decimal ) token.Value < int >() );

                case JTokenType.Float:
                    return new EngineRuntimeObject( ( decimal ) token.Value < float >() );

                case JTokenType.Boolean:
                    return new EngineRuntimeObject( token.Value < bool >() ? 1 : 0 );

                case JTokenType.Null:
                    return new EngineRuntimeObject( null );

                default:
                    throw new NotSupportedException( $"Can not convert type {token.Type}" );
            }
        }

        #endregion

    }

}
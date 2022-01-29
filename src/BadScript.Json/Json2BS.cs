using System.Collections.Generic;

using BadScript.Exceptions;
using BadScript.Parser.Expressions;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BadScript.Json
{

    internal static class Json2BS
    {

        #region Public

        public static ABSObject Convert( ABSObject[] args )
        {
            if(args.Length==2)return Populate(args);
            ABSObject o = args[0].ResolveReference();

            JToken jsonO = ( JToken )JsonConvert.DeserializeObject( o.ConvertString() );

            ABSObject ret = Convert( jsonO );

            return ret;
        }
        private static ABSObject Populate( ABSObject[] args )
        {
            ABSObject o = args[0].ResolveReference();
            ABSObject b = args[1].ResolveReference();

            JToken jsonO = ( JToken )JsonConvert.DeserializeObject( o.ConvertString() );

            ABSObject ret = Convert( jsonO, b );

            return ret;
        }

        #endregion

        #region Private

        private static ABSObject Convert( JToken jsonT, ABSObject baseObj =null )
        {
            if ( jsonT is JArray a )
            {
                BSArray arr = baseObj as BSArray;
                return Convert( a , arr);
            }

            if ( jsonT is JObject o )
            {
                BSTable table = baseObj as BSTable;

                return Convert( o, table );
            }

            

            if ( jsonT.Type == JTokenType.Boolean )
            {
                return jsonT.Value < bool >() ? BSObject.True : BSObject.False;
            }

            if ( jsonT.Type == JTokenType.Float || jsonT.Type == JTokenType.Integer )
            {
                return new BSObject( jsonT.Value < decimal >() );
            }

            return ConvertValue( jsonT );
        }

        private static ABSArray Convert( JArray jsonA, BSArray a = null )
        {
            a = a ?? new BSArray();

            for ( int i = 0; i < jsonA.Count; i++ )
            {
                JToken jToken = jsonA[i];
                a.InsertElement( i, Convert( jToken ) );
            }

            return a;
        }

        private static ABSObject Convert( JObject jsonO, BSTable t = null )
        {
            t = t ?? new BSTable( SourcePosition.Unknown );

            foreach ( KeyValuePair < string, JToken > kvp in jsonO )
            {
                ABSObject key = new BSObject( kvp.Key );
                if(!t.HasElement(key))
                    t.InsertElement(key , Convert( kvp.Value ) );
                else
                {
                    ABSObject o = t.GetRawElement( key );
                    t.InsertElement( key, Convert( kvp.Value, o ) );
                }
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
                    return new BSObject( ( decimal )token.Value < int >() );

                case JTokenType.Float:
                    return new BSObject( ( decimal )token.Value < float >() );

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

using System;

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

using Newtonsoft.Json.Linq;

namespace BadScript.Json
{

    internal static class BS2Json
    {

        #region Public

        public static ABSObject Convert( ABSObject[] args )
        {
            ABSObject o = args[0].ResolveReference();

            JToken obj = Convert( o );
            string str = obj.ToString();

            return new BSObject( str );
        }

        #endregion

        #region Private

        private static JToken Convert( ABSArray a )
        {
            JArray ret = new JArray();

            for ( int i = 0; i < a.GetLength(); i++ )
            {
                ret.Add( Convert( a.GetElement( i ) ) );
            }

            return ret;
        }

        private static JToken Convert( ABSTable t )
        {
            JObject o = new JObject();

            ABSArray keys = t.Keys;

            for ( int i = 0; i < keys.GetLength(); i++ )
            {
                ABSObject k = keys.GetElement( i ).ResolveReference();

                o.Add( k.ToString(), Convert( t.GetElement( k ) ) );
            }

            return o;
        }

        private static JToken Convert( ABSObject o )
        {
            o = o.ResolveReference();

            if ( o is ABSArray a )
            {
                return Convert( a );
            }

            if ( o is ABSTable t )
            {
                return Convert( t );
            }

            return ConvertValue( o );
        }

        private static JToken ConvertValue( ABSObject o )
        {
            o = o.ResolveReference();

            if ( o is BSObject obj )
            {
                JValue ret = new JValue( obj.GetInternalObject() );

                return ret;
            }

            throw new NotSupportedException( $"Can not convert type {o.GetType()}" );
        }

        #endregion

    }

}

using System;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

using Newtonsoft.Json.Linq;

namespace BadScript.Json
{

    internal static class BS2Json
    {

        #region Public

        public static BSRuntimeObject Convert( BSRuntimeObject[] args )
        {
            BSRuntimeObject o = args[0];

            while ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            JToken obj = Convert( o );
            string str = obj.ToString();

            return new EngineRuntimeObject( str );
        }

        #endregion

        #region Private

        private static JToken Convert( BSRuntimeArray a )
        {
            JArray ret = new JArray();

            for ( int i = 0; i < a.GetLength(); i++ )
            {
                ret.Add( Convert( a.GetElement( i ) ) );
            }

            return ret;
        }

        private static JToken Convert( BSRuntimeTable t )
        {
            JObject o = new JObject();

            BSRuntimeArray keys = t.Keys;

            for ( int i = 0; i < keys.GetLength(); i++ )
            {
                BSRuntimeObject k = keys.GetElement( i );

                while ( k is BSRuntimeReference r )
                {
                    k = r.Get();
                }

                o.Add( k.ToString(), Convert( t.GetElement( k ) ) );
            }

            return o;
        }

        private static JToken Convert( BSRuntimeObject o )
        {
            while ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o is BSRuntimeArray a )
            {
                return Convert( a );
            }

            if ( o is BSRuntimeTable t )
            {
                return Convert( t );
            }

            return ConvertValue( o );
        }

        private static JToken ConvertValue( BSRuntimeObject o )
        {
            while ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o is EngineRuntimeObject obj )
            {
                JValue ret = new JValue( obj.GetInternalObject() );

                return ret;
            }

            throw new NotSupportedException( $"Can not convert type {o.GetType()}" );
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BadScript.Serialization
{

    public class BSSerializerContext
    {

        private List < string > m_StringCache = new List < string >();

        #region Public

        public static BSSerializerContext Deserialize( Stream s )
        {
            List < string > cache = s.DeserializeStringArray( null ).ToList();
#if DEBUG
            Console.WriteLine( "Cached Strings: " + cache.Count );
#endif
            return new BSSerializerContext { m_StringCache = cache };
        }

        public int AddCached( string s )
        {
            int idx = m_StringCache.IndexOf( s );

            if ( idx == -1 )
            {
                m_StringCache.Add( s );

                return m_StringCache.Count - 1;
            }

            return idx;
        }

        public string ResolveCached( int idx )
        {
            return m_StringCache[idx];
        }

        public void Serialize( Stream s )
        {
#if DEBUG
            Console.WriteLine( "Cached Strings: " + m_StringCache.Count );
#endif

            s.SerializeStringArray( m_StringCache.ToArray(), null );
        }

        #endregion

    }

}

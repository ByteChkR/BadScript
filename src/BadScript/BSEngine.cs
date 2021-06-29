using System.Collections.Generic;
using BadScript.Common.Types;

namespace BadScript
{

    public class BSEngine
    {
        private static BSEngine s_Instance;

        private readonly Dictionary < string, ABSObject > m_StaticData =
            new Dictionary < string, ABSObject >();

        #region Public

        public static void AddStatic( string k, ABSObject o )
        {
            GetInstance().AddStaticData( k, o );
        }

        public static BSEngineInstance CreateEngineInstance()
        {
            return new BSEngineInstance(
                new Dictionary < string, ABSObject >(
                    GetInstance().m_StaticData
                )
            );
        }

        public void AddStaticData( string k, ABSObject o )
        {
            m_StaticData[k] = o;
        }

        public object GetPluginInstance()
        {
            return this;
        }

        #endregion

        #region Private

        private static BSEngine GetInstance()
        {
            return s_Instance ?? ( s_Instance = new BSEngine() );
        }

        #endregion
    }

}

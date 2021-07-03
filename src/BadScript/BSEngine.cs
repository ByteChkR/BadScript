using System;
using System.Collections.Generic;
using System.IO;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

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

        private static void LoadDirectory(  BSEngineInstance instance, string dir )
        {
            foreach ( string file in Directory.GetFiles(dir, "*.bs", SearchOption.TopDirectoryOnly) )
            {
                Console.WriteLine( "Loading File: " + file );
                instance.LoadFile( file, new string[0] );
            }
        }

        public static BSEngineInstance CreateEngineInstance(params string[] includeDirs)
        {
            Dictionary < string, ABSObject > data = new Dictionary < string, ABSObject >(
                GetInstance().m_StaticData
            );

            Dictionary < ABSObject, ABSObject > sData = new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, ABSObject > keyValuePair in data )
            {
                sData[new BSObject(keyValuePair.Key)] = keyValuePair.Value;
            }
            BSEngineInstance instance = new BSEngineInstance(data);
            

            foreach (string includeDir in includeDirs)
            {
                LoadDirectory(instance, includeDir);
            }

            return instance;
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

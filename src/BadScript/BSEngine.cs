using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript
{

    public class BSEngine
    {
        private static BSEngine s_Instance;

        private readonly List < ABSScriptInterface > m_Interfaces = new List < ABSScriptInterface >();

        #region Public

        public static void AddStatic( ABSScriptInterface i )
        {
            GetInstance().AddStaticInterface( i );
        }

        public static BSEngineInstance CreateEngineInstance( string[] staticInterfaces, params string[] includeDirs )
        {
            List < ABSScriptInterface > interfaces = GetInstance().GetInterfaceList();
            (bool useGlobal, string interfaceName)[] data = ParseInterfaceNames( staticInterfaces );

            Dictionary < string, ABSTable > interfaceData =
                GetInterfaceData( FindInterfaces( false, interfaces, data ) );

            Dictionary < string, ABSObject > interfaceObjects =
                interfaceData.ToDictionary( x => x.Key, x => ( ABSObject ) x.Value );

            BSEngineInstance instance = new BSEngineInstance( interfaceObjects, interfaces );

            AddGlobalInterfaces( instance, FindInterfaces( true, interfaces, data ) );

            foreach ( string includeDir in includeDirs )
            {
                LoadDirectory( instance, includeDir );
            }

            return instance;
        }

        public static string[] GetDefaultInterfaces()
        {
            return new[] { "#core" };
        }

        public static Dictionary < string, ABSTable > GetInterfaceData( List < ABSScriptInterface > interfaces )
        {
            Dictionary < string, ABSTable > o = new Dictionary < string, ABSTable >();

            foreach ( ABSScriptInterface bsScriptInterface in interfaces )
            {

                ABSTable t;

                if ( !o.TryGetValue( bsScriptInterface.Name, out t ) )
                {
                    t = new BSTable( SourcePosition.Unknown );
                    o[bsScriptInterface.Name] = t;
                }

                bsScriptInterface.AddApi( t );
            }

            return o;
        }

        public void AddStaticInterface( ABSScriptInterface i )
        {
            m_Interfaces.Add( i );
        }

        public List < ABSScriptInterface > GetInterfaceList()
        {
            return new List < ABSScriptInterface >( m_Interfaces );
        }

        #endregion

        #region Private

        private static void AddGlobalInterfaces( BSEngineInstance instance, List < ABSScriptInterface > interfaces )
        {
            ABSTable root = instance.GetGlobalTable();

            foreach ( ABSScriptInterface absScriptInterface in interfaces )
            {
                absScriptInterface.AddApi( root );
            }
        }

        private static List < ABSScriptInterface > FindInterfaces(
            bool global,
            List < ABSScriptInterface > interfaces,
            (bool useGlobal, string interfaceName) key )
        {
            return interfaces.Where( x => x.Name == key.interfaceName && global == key.useGlobal ).ToList();
        }

        private static List < ABSScriptInterface > FindInterfaces(
            bool global,
            List < ABSScriptInterface > interfaces,
            (bool useGlobal, string interfaceName)[] keys )
        {
            return interfaces.Where( x => keys.Any( y => y.interfaceName == x.Name && global == y.useGlobal ) ).
                              ToList();
        }

        private static BSEngine GetInstance()
        {
            return s_Instance ?? ( s_Instance = new BSEngine() );
        }

        private static void LoadDirectory( BSEngineInstance instance, string dir )
        {
            foreach ( string file in Directory.GetFiles( dir, "*.bs", SearchOption.TopDirectoryOnly ) )
            {
                Console.WriteLine( "Loading File: " + file );
                instance.LoadFile( file, new string[0] );
            }
        }

        private static (bool useGlobalTable, string interfaceName)[] ParseInterfaceNames( string[] names )
        {
            List < (bool, string) > data = new List < (bool, string) >();

            foreach ( string name in names )
            {
                (bool useGlobal, string intName) d = ( false, name );

                if ( name.StartsWith( "#" ) )
                {
                    d.useGlobal = true;
                    d.intName = name.Remove( 0, 1 );
                }

                data.Add( d );
            }

            return data.ToArray();
        }

        #endregion
    }

}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Expressions;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Interfaces;

namespace BadScript.Settings
{

    public class BSEngineSettings
    {
        public readonly BSParserSettings ParserSettings;
        public readonly List < ABSScriptInterface > Interfaces;
        public readonly List < string > ActiveInterfaces;
        public readonly List < string > IncludeDirectories;

        public ABSScriptInterface[] ActiveGlobalInterfaces => FindInterfaces(
                true,
                Interfaces,
                ParseInterfaceNames( ActiveInterfaces ) ).
            ToArray();
        public ABSScriptInterface[] ActiveLocalInterfaces => FindInterfaces(
                false,
                Interfaces,
                ParseInterfaceNames( ActiveInterfaces ) ).
            ToArray();

        #region Public

        public BSEngineSettings( BSParserSettings parserSettings )
        {
            Interfaces = new List < ABSScriptInterface >();
            ActiveInterfaces = new List < string >();
            IncludeDirectories = new List < string >();
            ParserSettings = parserSettings;
        }

        public static BSEngineSettings MakeDefault( BSParserSettings parserSettings )
        {
            BSEngineSettings s = new BSEngineSettings( parserSettings );
            s.ActiveInterfaces.Add( "#core" );
            s.ActiveInterfaces.Add( "#console" );

            return s;
        }

        public BSEngine Build()
        {
            BSEngine instance = new BSEngine(
                ParserSettings,
                GetInterfaceData( ActiveLocalInterfaces ),
                Interfaces );

            AddGlobalInterfaces( instance, ActiveGlobalInterfaces );

            foreach ( string includeDir in IncludeDirectories )
            {
                LoadDirectory( instance, includeDir );
            }

            return instance;
        }

        #endregion

        #region Private

        private static void AddGlobalInterfaces( BSEngine instance, IEnumerable < ABSScriptInterface > interfaces )
        {
            ABSTable g = instance.GetGlobalTable();

            foreach ( ABSScriptInterface absScriptInterface in interfaces )
            {
                absScriptInterface.AddApi( g );
            }
        }

        private static List < ABSScriptInterface > FindInterfaces(
            bool global,
            List < ABSScriptInterface > interfaces,
            (bool useGlobal, string interfaceName)[] keys )
        {
            return interfaces.Where( x => keys.Any( y => y.interfaceName == x.Name && global == y.useGlobal ) ).
                              ToList();
        }

        private static Dictionary < string, ABSObject > GetInterfaceData(
            IEnumerable < ABSScriptInterface > interfaces )
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

            return o.ToDictionary( x => x.Key, x => ( ABSObject ) x.Value );
        }

        private static void LoadDirectory( BSEngine instance, string dir )
        {
            foreach ( string file in Directory.GetFiles( dir, "*.bs", SearchOption.TopDirectoryOnly ) )
            {
                instance.LoadFile( file, new[] { dir }, false );
            }
        }

        private static (bool useGlobalTable, string interfaceName)[] ParseInterfaceNames( List < string > names )
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

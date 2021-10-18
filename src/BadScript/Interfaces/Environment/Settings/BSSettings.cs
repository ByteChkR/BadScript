using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Settings;

namespace BadScript.Interfaces.Environment.Settings
{

    public static class BSSettings
    {

        private static SettingsCategory s_BsRoot;

        public static SettingsCategory BsRoot
        {
            get => s_BsRoot;
            set
            {
                s_BsRoot = value;
                SettingsCategory runtime = s_BsRoot.AddCategory( "runtime" );
                runtime.SetSetting( "version", typeof( BSEngine ).Assembly.GetName().Version.ToString() );
            }
        }

        #region Public

        public static void LoadFromDirectory( this SettingsCategory cat, string directory )
        {
            if ( !Directory.Exists( directory ) )
            {
                Directory.CreateDirectory( directory );
            }

            string[] subs = Directory.GetDirectories( directory, "*", SearchOption.TopDirectoryOnly ).
                                      Select( Path.GetFileName ).
                                      ToArray();

            string[] pairs = Directory.GetFiles( directory, "*.setting", SearchOption.TopDirectoryOnly ).
                                       Select( Path.GetFileName ).
                                       ToArray();

            foreach ( string sub in subs )
            {
                string p = Path.Combine( directory, sub );
                SettingsCategory scat = cat.AddCategory( sub );
                LoadFromDirectory( scat, p );
            }

            foreach ( string pair in pairs )
            {
                string p = Path.Combine( directory, pair );
                cat.SetSetting( Path.GetFileNameWithoutExtension( pair ), File.ReadAllText( p ) );
            }
        }

        public static void SaveToDirectory( this SettingsCategory cat, string directory )
        {
            foreach ( SettingsCategory sub in cat )
            {
                if ( !sub.IsPersistent )
                {
                    continue;
                }

                string p = Path.Combine( directory, sub.Name );
                Directory.CreateDirectory( p );
                sub.SaveToDirectory( p );
            }

            foreach ( SettingsPair pair in ( IEnumerable < SettingsPair > )cat )
            {
                if ( !pair.IsPersistent )
                {
                    continue;
                }

                string p = Path.Combine( directory, pair.Name + ".setting" );
                File.WriteAllText( p, pair.Value );
            }
        }

        #endregion

        #region Private

        static BSSettings()
        {
            BsRoot = SettingsCategory.CreateRoot( "bs" );
        }

        #endregion

    }

}

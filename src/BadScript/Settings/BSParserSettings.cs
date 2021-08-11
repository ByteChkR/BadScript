using System.Collections.Generic;
using System.IO;
using BadScript.Utils;

namespace BadScript.Settings
{

    public static class BSSettings
    {
        public static SettingsCategory BsRoot { get; }

        static BSSettings()
        {
            BsRoot = SettingsRoot.Root.AddCategory( "bs" );
            ParserCategory = BsRoot.AddCategory("parser");
            RuntimeCategory = BsRoot.AddCategory("runtime");
        }



        public static SettingsCategory ParserCategory { get; }
        public static SettingsCategory RuntimeCategory { get; }
        

        public static void SaveToDirectory(this SettingsCategory cat, string directory)
        {

            foreach ( SettingsCategory sub in cat )
            {
                string p = Path.Combine( directory, sub.Name );
                Directory.CreateDirectory( p );
                sub.SaveToDirectory(p);
            }

            foreach ( SettingsPair pair in (IEnumerable < SettingsPair > )cat )
            {
                string p = Path.Combine( directory, pair.Name+".setting" );
                File.WriteAllText( p, pair.Value );
            }
        }

        public static void LoadFromDirectory(this SettingsCategory cat, string directory)
        {
            if ( !Directory.Exists( directory ) )
                Directory.CreateDirectory( directory );
            string[] subs = Directory.GetDirectories( directory, "*", SearchOption.TopDirectoryOnly );
            string[] pairs = Directory.GetFiles( directory, "*.setting", SearchOption.TopDirectoryOnly );

            foreach ( string sub in subs )
            {
                string p = Path.Combine( directory, sub );
                LoadFromDirectory(cat.AddCategory(sub), p);
            }

            foreach ( string pair in pairs )
            {
                string p = Path.Combine(directory, pair+".setting");
                cat.SetSetting( pair, File.ReadAllText( p ) );
            }
        }

    }

    public class BSParserSettings
    {
        public bool AllowOptimization;

        public static BSParserSettings Default => new BSParserSettings { AllowOptimization = true };

        #region Public

        public BSParserSettings()
        {

        }

        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Utils;

namespace BadScript.Settings
{

    public static class BSSettings
    {
        public static SettingsCategory BsRoot { get; }

        static BSSettings()
        {
            BsRoot = SettingsCategory.CreateRoot( "bs" );// SettingsRoot.Root.AddCategory( "bs" );
            ParserCategory = BsRoot.AddCategory("parser");
            RuntimeCategory = BsRoot.AddCategory("runtime");
        }



        public static SettingsCategory ParserCategory { get; }
        public static SettingsCategory RuntimeCategory { get; }
        

        public static void SaveToDirectory(this SettingsCategory cat, string directory)
        {

            foreach ( SettingsCategory sub in cat )
            {
                if (!sub.IsPersistent) continue;
                string p = Path.Combine( directory, sub.Name );
                Directory.CreateDirectory( p );
                sub.SaveToDirectory(p);
            }

            foreach ( SettingsPair pair in (IEnumerable < SettingsPair > )cat )
            {
                if(!pair.IsPersistent)continue;
                string p = Path.Combine( directory, pair.Name+".setting" );
                File.WriteAllText( p, pair.Value );
            }
        }

        public static void LoadFromDirectory(this SettingsCategory cat, string directory)
        {
            if ( !Directory.Exists( directory ) )
                Directory.CreateDirectory( directory );
            string[] subs = Directory.GetDirectories( directory, "*", SearchOption.TopDirectoryOnly ).Select(Path.GetFileName).ToArray();
            string[] pairs = Directory.GetFiles( directory, "*.setting", SearchOption.TopDirectoryOnly ).Select(Path.GetFileName).ToArray();

            foreach ( string sub in subs )
            {
                string p = Path.Combine( directory, sub );
                SettingsCategory scat = cat.AddCategory( sub );
                LoadFromDirectory(scat, p);
            }

            foreach ( string pair in pairs )
            {
                string p = Path.Combine(directory, pair);
                SettingsPair spair = cat.SetSetting( Path.GetFileNameWithoutExtension(pair), File.ReadAllText( p ) );
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

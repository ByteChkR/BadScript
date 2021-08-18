using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using BadScript.Utils;

namespace BadScript.Settings
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
                ParserCategory = s_BsRoot.AddCategory( "parser" );
                RuntimeCategory = s_BsRoot.AddCategory( "runtime" );
            }
        }

        public static SettingsCategory ParserCategory { get; private set; }

        public static SettingsCategory RuntimeCategory { get; private set; }

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
                SettingsPair spair = cat.SetSetting( Path.GetFileNameWithoutExtension( pair ), File.ReadAllText( p ) );
            }
        }

        public static void LoadFromZip( this SettingsCategory cat, string file )
        {
            //TODO Load directly from ZIP File.
            string tempDir = Path.Combine( Path.GetTempPath(), Path.GetRandomFileName() );
            Directory.CreateDirectory( tempDir );
            ZipFile.ExtractToDirectory( file, tempDir );
            cat.LoadFromDirectory( tempDir );
            Directory.Delete( tempDir, true );
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

            foreach ( SettingsPair pair in ( IEnumerable < SettingsPair > ) cat )
            {
                if ( !pair.IsPersistent )
                {
                    continue;
                }

                string p = Path.Combine( directory, pair.Name + ".setting" );
                File.WriteAllText( p, pair.Value );
            }
        }

        public static void SaveToZip( this SettingsCategory cat, string file )
        {
            using MemoryStream memoryStream = new MemoryStream();

            using ( ZipArchive archive = new ZipArchive( memoryStream, ZipArchiveMode.Create, true ) )
            {
                SaveToZip( archive, cat, "" );
            }

            using ( FileStream fileStream = new FileStream( file, FileMode.Create ) )
            {
                memoryStream.Seek( 0, SeekOrigin.Begin );
                memoryStream.CopyTo( fileStream );
            }
        }

        #endregion

        #region Private

        static BSSettings()
        {
            BsRoot = SettingsCategory.CreateRoot( "bs" ); // SettingsRoot.Root.AddCategory( "bs" );
        }

        private static void SaveToZip( ZipArchive za, SettingsCategory cat, string directory )
        {
            foreach ( SettingsCategory sub in cat )
            {
                if ( !sub.IsPersistent )
                {
                    continue;
                }

                string p = Path.Combine( directory, sub.Name );
                SaveToZip( za, sub, p );
            }

            foreach ( SettingsPair pair in ( IEnumerable < SettingsPair > ) cat )
            {
                if ( !pair.IsPersistent )
                {
                    continue;
                }

                string p = Path.Combine( directory, pair.Name + ".setting" );

                ZipArchiveEntry fileEntry = za.CreateEntry( p );
                using Stream entryStream = fileEntry.Open();

                using StreamWriter streamWriter = new StreamWriter( entryStream );

                streamWriter.Write( pair.Value );

            }
        }

        #endregion
    }

}

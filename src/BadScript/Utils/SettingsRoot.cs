using System;

namespace BadScript.Utils
{

    public class SettingsRoot : SettingsCategory
    {
        public static readonly SettingsRoot Root = new SettingsRoot();

        #region Public

        internal SettingsRoot() : base( "_" )
        {

        }

        public static SettingsCategory FindCategory( string categoryName )
        {
            string[] parts = categoryName.Split( '.' );
            SettingsCategory current = Root;

            for ( int i = 0; i < parts.Length; i++ )
            {
                if ( current.HasCategory( parts[i] ) )
                {
                    current = current.GetCategory( parts[i] );
                }
                else
                {
                    throw new Exception( "Can not Find Settings: " + categoryName );
                }
            }

            return current;
        }

        public static SettingsPair GetSetting( string categoryName, string setting )
        {
            SettingsCategory c = FindCategory( categoryName );

            return c.GetSetting( setting );
        }

        public static void SetSetting( string categoryName, string setting, string value )
        {
            SettingsCategory c = FindCategory( categoryName );
            c.SetSetting( setting, value );
        }

        #endregion
    }

}

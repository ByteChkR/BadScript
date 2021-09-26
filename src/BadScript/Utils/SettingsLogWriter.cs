using System;

namespace BadScript.Utils
{

    public class SettingsLogWriter
    {

        [Flags]
        public enum SettingsLogSettings
        {

            None = 0,
            OnAddCategory = 1,
            OnAddPair = 2,
            OnValueChanged = 4,
            OnPersistenceChanged = 8,
            All = -1

        }

        private readonly Action < string > m_Write;
        private SettingsLogSettings m_Settings;

        #region Public

        public SettingsLogWriter( Action < string > cout, SettingsLogSettings settings )
        {
            m_Write = cout;
            m_Settings = settings;
            SettingsCategory.OnCategoryAdd += BSSettings_OnAddCategory;
            SettingsCategory.OnSettingsAdd += BSSettings_OnAddPair;
        }

        #endregion

        #region Private

        private void BSSettings_OnAddCategory( SettingsCategory parent, SettingsCategory category )
        {
            if ( ( m_Settings & SettingsLogSettings.OnAddCategory ) != 0 )
            {
                m_Write( $"Loading Category: {category}" );
            }
        }

        private void BSSettings_OnAddPair( SettingsCategory category, SettingsPair pair )
        {
            if ( ( m_Settings & SettingsLogSettings.OnAddPair ) != 0 )
            {
                m_Write( $"Loading Setting: {category}.{pair.Name}" );
            }

            pair.OnValueChanged += p => SettingsPair_OnValueChanged( category, p );
            pair.OnPersistenceChanged += p => SettingsPair_OnPersistenceChanged( category, p );
        }

        private void SettingsPair_OnPersistenceChanged( SettingsCategory category, SettingsPair pair )
        {
            if ( ( m_Settings & SettingsLogSettings.OnPersistenceChanged ) != 0 )
            {
                m_Write( $"Changed Setting Persistence: {category}.{pair}" );
            }
        }

        private void SettingsPair_OnValueChanged( SettingsCategory category, SettingsPair pair )
        {
            if ( ( m_Settings & SettingsLogSettings.OnValueChanged ) != 0 )
            {
                m_Write( $"Changed Setting Value: {category}.{pair}" );
            }
        }

        #endregion

    }

}

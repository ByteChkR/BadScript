using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BadScript.Utils
{

    public class SettingsCategory : IEnumerable < SettingsCategory >, IEnumerable <SettingsPair>
    {
        public readonly string Name;
        private readonly List < SettingsCategory > m_SubCategories;
        private readonly List < SettingsPair > m_Settings;

        #region Public

        public static SettingsCategory CreateRoot()
        {
            return new SettingsRoot();
        }



        public void AddCategory(SettingsCategory c)
        {
            if (!m_SubCategories.Contains(c))
            {
                m_SubCategories.Add(c);
            }
        }

        public SettingsCategory AddCategory(string c)
        {
            SettingsCategory r = null;
            if (!HasCategory(c))
            {
                m_SubCategories.Add(r = new SettingsCategory(c));
            }
            else
            {
                r = GetCategory(c);
            }

            return r;
        }

        public SettingsCategory GetCategory( string name )
        {
            return m_SubCategories.First( x => x.Name == name );
        }

        IEnumerator < SettingsPair > IEnumerable < SettingsPair >.GetEnumerator()
        {
            return m_Settings.GetEnumerator();
        }

        public IEnumerator < SettingsCategory > GetEnumerator()
        {
            return m_SubCategories.GetEnumerator();
        }

        public SettingsPair GetSetting( string name )
        {
            return m_Settings.First( x => x.Name == name );
        }

        public bool HasCategory( string name )
        {
            return m_SubCategories.Any( x => x.Name == name );
        }

        public bool HasSetting( string name )
        {
            return m_Settings.Any( x => x.Name == name );
        }

        public void SetSetting( string name, string value )
        {
            if ( HasSetting( name ) )
            {
                SettingsPair s = GetSetting( name );
                s.Value = value;
            }
            else
            {
                m_Settings.Add( new SettingsPair( name, value ) );
            }
        }

        #endregion

        #region Protected

        protected SettingsCategory( string name )
        {
            Name = name;
            m_Settings = new List < SettingsPair >();
            m_SubCategories = new List < SettingsCategory >();
        }

        #endregion

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_SubCategories ).GetEnumerator();
        }

        #endregion
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BadScript.Utils
{

    public class SettingsCategory : IEnumerable < SettingsCategory >, IEnumerable < SettingsPair >
    {
        public readonly string Name;
        public bool IsPersistent = true;

        private readonly SettingsCategory m_Parent;
        private readonly List < SettingsCategory > m_SubCategories;
        private readonly List < SettingsPair > m_Settings;

        public static event Action < SettingsCategory, SettingsPair > OnSettingsChanged;

        public static event Action < SettingsCategory, SettingsPair > OnSettingsAdd;

        public static event Action < SettingsCategory, SettingsCategory > OnCategoryAdd;

        public string FullName => m_Parent == null ? Name : m_Parent.FullName + "." + Name;

        #region Public

        public static SettingsCategory CreateRoot( string name )
        {
            return new SettingsCategory( name, null );
        }

        public void AddCategory( SettingsCategory c )
        {
            if ( !m_SubCategories.Contains( c ) )
            {
                OnCategoryAdd?.Invoke( this, c );
                m_SubCategories.Add( c );
            }
        }

        public SettingsCategory AddCategory( string c )
        {
            SettingsCategory r = null;

            if ( !HasCategory( c ) )
            {
                r = new SettingsCategory( c, this );
                AddCategory( r );
            }
            else
            {
                r = GetCategory( c );
            }

            return r;
        }

        public SettingsCategory GetCategory( string name )
        {
            return m_SubCategories.First( x => x.Name == name );
        }

        public IEnumerator < SettingsCategory > GetEnumerator()
        {
            return m_SubCategories.GetEnumerator();
        }

        public SettingsPair GetSetting( string name, string defaultValue )
        {
            if ( HasSetting( name ) )
            {
                return GetSetting( name );
            }

            return SetSetting( name, defaultValue );
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

        public SettingsPair SetSetting( string name, string value )
        {
            SettingsPair s = null;

            if ( HasSetting( name ) )
            {
                s = GetSetting( name );
                s.Value = value;
            }
            else
            {
                s = new SettingsPair( name, value );

                m_Settings.Add( s );
                OnSettingsAdd?.Invoke( this, s );
            }

            OnSettingsChanged?.Invoke( this, s );

            return s;
        }

        public override string ToString()
        {
            return FullName;
        }

        #endregion

        #region Protected

        protected SettingsCategory( string name, SettingsCategory parent )
        {
            Name = name;
            m_Parent = parent;
            m_Settings = new List < SettingsPair >();
            m_SubCategories = new List < SettingsCategory >();
        }

        #endregion

        #region Private

        IEnumerator < SettingsPair > IEnumerable < SettingsPair >.GetEnumerator()
        {
            return m_Settings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_SubCategories ).GetEnumerator();
        }

        #endregion
    }

}

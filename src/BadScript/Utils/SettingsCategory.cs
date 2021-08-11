﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

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

        private readonly Action <string> m_Write;
        private SettingsLogSettings m_Settings;
        public SettingsLogWriter(Action<string> cout, SettingsLogSettings settings)
        {
            m_Write = cout;
            m_Settings = settings;
            SettingsCategory.OnCategoryAdd += BSSettings_OnAddCategory;
            SettingsCategory.OnSettingsAdd += BSSettings_OnAddPair;
        }


        private void BSSettings_OnAddPair(SettingsCategory category, SettingsPair pair)
        {
            if ((m_Settings & SettingsLogSettings.OnAddPair) != 0)
                m_Write($"Loading Setting: {category}.{pair.Name}");
            pair.OnValueChanged += p => SettingsPair_OnValueChanged(category, p);
            pair.OnPersistenceChanged += p => SettingsPair_OnPersistenceChanged(category, p);
        }

        private void BSSettings_OnAddCategory(SettingsCategory parent, SettingsCategory category)
        {
            if ((m_Settings & SettingsLogSettings.OnAddCategory) != 0)
                m_Write($"Loading Category: {category}");
        }

        private void SettingsPair_OnValueChanged(SettingsCategory category, SettingsPair pair)
        {
            if ((m_Settings & SettingsLogSettings.OnValueChanged) != 0)
                m_Write($"Changed Setting Value: {category}.{pair}");
        }

        private void SettingsPair_OnPersistenceChanged(SettingsCategory category, SettingsPair pair)
        {
            if ((m_Settings & SettingsLogSettings.OnPersistenceChanged) != 0)
                m_Write($"Changed Setting Persistence: {category}.{pair}");
        }
    }

    public class SettingsCategory : IEnumerable < SettingsCategory >, IEnumerable <SettingsPair>
    {
        public static event Action<SettingsCategory, SettingsPair> OnSettingsChanged;
        public static event Action<SettingsCategory, SettingsPair> OnSettingsAdd;
        public static event Action<SettingsCategory, SettingsCategory> OnCategoryAdd;

        public readonly string Name;
        public bool IsPersistent = true;

        private readonly SettingsCategory m_Parent;
        private readonly List < SettingsCategory > m_SubCategories;
        private readonly List < SettingsPair > m_Settings;

        #region Public

        public string FullName => m_Parent == null ? Name : m_Parent.FullName + "." + Name;

        public override string ToString()
        {
            return FullName;
        }


        public static SettingsCategory CreateRoot( string name ) => new SettingsCategory( name, null );

        public void AddCategory(SettingsCategory c)
        {
            if (!m_SubCategories.Contains(c))
            {
                OnCategoryAdd?.Invoke( this, c );
                m_SubCategories.Add(c);
            }
        }

        public SettingsCategory AddCategory(string c)
        {
            SettingsCategory r = null;
            if (!HasCategory(c))
            {
                r = new SettingsCategory( c, this );
                AddCategory( r );
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

        public SettingsPair GetSetting(string name, string defaultValue)
        {
            if ( HasSetting( name ) )
                return GetSetting( name );

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

                m_Settings.Add( s);
                OnSettingsAdd?.Invoke(this, s);
            }

            OnSettingsChanged?.Invoke( this, s );

            return s;
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_SubCategories ).GetEnumerator();
        }

        #endregion
    }

}

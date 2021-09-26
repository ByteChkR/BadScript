using System;

using Newtonsoft.Json;

namespace BadScript.Utility
{

    public class SettingsPair
    {

        public readonly string Name;
        private string m_Value;
        private bool m_IsPersistent = true;

        public event Action < SettingsPair > OnValueChanged;

        public event Action < SettingsPair > OnPersistenceChanged;

        public string Value
        {
            get => m_Value;
            set
            {
                bool changed = m_Value != value;
                m_Value = value;

                if ( changed )
                {
                    OnValueChanged?.Invoke( this );
                }
            }
        }

        public bool IsPersistent
        {
            get => m_IsPersistent;
            set
            {
                bool changed = m_IsPersistent != value;
                m_IsPersistent = value;

                if ( changed )
                {
                    OnPersistenceChanged?.Invoke( this );
                }
            }
        }

        public static implicit operator bool( SettingsPair p )
        {
            return bool.Parse( p.Value );
        }

        public static implicit operator decimal( SettingsPair p )
        {
            return decimal.Parse( p.Value );
        }

        #region Public

        public SettingsPair( string name, string value )
        {
            Name = name;
            Value = value;
        }

        public T ReadJson < T >()
        {
            return JsonConvert.DeserializeObject < T >( Value );
        }

        public override string ToString()
        {
            return $"{Name}(Persistent: {IsPersistent}): {Value}";
        }

        public void WriteJson( object obj )
        {
            Value = JsonConvert.SerializeObject( obj );
        }

        #endregion

    }

}

using Newtonsoft.Json;

namespace BadScript.Utils
{

    public class SettingsPair
    {
        public readonly string Name;
        public string Value;

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

        public void WriteJson( object obj )
        {
            Value = JsonConvert.SerializeObject( obj );
        }

        #endregion
    }

}

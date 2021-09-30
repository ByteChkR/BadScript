namespace BadScript.Utils
{
    public class SimpleSingleton<T> where T: SimpleSingleton <T>, new()
    {

        private static T s_Instance;
        public static T Instance => s_Instance ?? InitializeIfNull();

        public static T InitializeIfNull()
        {
            if ( s_Instance == null )
                s_Instance = new T();

            return s_Instance;
        }

    }

}

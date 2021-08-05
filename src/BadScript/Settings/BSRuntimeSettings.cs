namespace BadScript.Settings
{

    public class BSRuntimeSettings
    {
        public string[] DefaultInterfaces;

        public static BSRuntimeSettings Default =>
            new BSRuntimeSettings { DefaultInterfaces = new[] { "#core", "#console" } };
    }

}

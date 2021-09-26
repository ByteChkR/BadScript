using BadScript.Console.Subsystems.Project.Utils;

namespace BadScript.Console.Subsystems.Project.DataObjects
{

    public class AppInfo : ReflectedObject
    {

        public string Name = "App";
        public string Version = "0.0.0.1";

        #region Public

        public AppInfo()
        {
        }

        public AppInfo( string name, string version = null )
        {
            Name = name;
            Version = version ?? Version;
        }

        #endregion

    }

}

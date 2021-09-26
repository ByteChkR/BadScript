using BadScript.Console.Subsystems.Project.DataObjects;

namespace BadScript.Console.Subsystems.Project.Utils
{

    public class ReflectionResolveInfo
    {

        public string CurrentTarget;
        public ProjectSettings Settings;

        #region Public

        public ReflectionResolveInfo( string target, ProjectSettings settings )
        {
            CurrentTarget = target;
            Settings = settings;
        }

        #endregion

    }

}

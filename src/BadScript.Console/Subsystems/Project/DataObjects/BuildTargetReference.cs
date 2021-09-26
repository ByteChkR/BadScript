using System.IO;

using BadScript.Console.Subsystems.Project.Utils;

namespace BadScript.Console.Subsystems.Project.DataObjects
{

    public class BuildTargetReference : ReflectedObject
    {

        public string Target = "build";
        public string Path = "<path-to-reference>";

        #region Public

        public BuildTargetReference()
        {
        }

        public BuildTargetReference( string target, string path )
        {
            Target = target;
            Path = path;
        }

        public ProjectSettings GetSettings( ProjectSettings s, string target )
        {
            return ProjectSettings.Deserialize( File.ReadAllText( s.ResolveValue( Path, target ) ) );
        }

        #endregion

    }

}

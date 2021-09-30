using System.Collections.Generic;

using BadScript.Console.Subsystems.Project.Utils;

namespace BadScript.Console.Subsystems.Project.DataObjects
{

    public class BuildTarget : ReflectedObject
    {

        public string Name = "build";
        public string OutputFormat = "text";

        public string OutputFile =
            "./bin/%Target.Name%/%AppInfo.Name%%AppInfo.Version%.%Target.Output.OutputExtension%";
        public string PreprocessorDirectives = "";
        public List < string > Include = new();
        public string SubTarget = "";
        public List < BuildTargetReference > References = new();
        public List < string > PreEvents = new();
        public List < string > PostEvents = new();

        #region Public

        public BuildTarget()
        {
        }

        public BuildTarget( string name )
        {
            Name = name;
        }

        public override string ResolveProperty( int current, string[] parts, ReflectionResolveInfo info )
        {
            if ( parts[current] == "Output" )
            {
                return ProjectSettings.GetOutputFormat( OutputFormat ).ResolveProperty( current + 1, parts, info );
            }

            return base.ResolveProperty( current, parts, info );
        }

        #endregion

    }

}

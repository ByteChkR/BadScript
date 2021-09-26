using BadScript.Console.Subsystems.Project.DataObjects;
using BadScript.Console.Subsystems.Project.Utils;

namespace BadScript.Console.Subsystems.Project.BuildFormats
{

    public abstract class BuildOutputFormat : ReflectedObject
    {

        public readonly string Name;
        public readonly string OutputExtension;

        #region Public

        public abstract void BuildOutput( BuildTarget t, string src, string outputFile );

        #endregion

        #region Protected

        protected BuildOutputFormat( string name, string ext )
        {
            OutputExtension = ext;
            Name = name;
        }

        #endregion

    }

}

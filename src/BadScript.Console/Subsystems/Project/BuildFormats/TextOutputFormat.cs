using System.IO;

using BadScript.Console.Subsystems.Project.DataObjects;

namespace BadScript.Console.Subsystems.Project.BuildFormats
{

    public class TextOutputFormat : BuildOutputFormat
    {

        #region Public

        public TextOutputFormat() : base( "text", "bs" )
        {
        }

        public override void BuildOutput( BuildTarget t, string src, string outputFile )
        {
            File.WriteAllText( outputFile, src );
        }

        #endregion

    }

}

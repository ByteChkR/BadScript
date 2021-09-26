using System.IO;

using BadScript.Common;
using BadScript.Console.Subsystems.Project.DataObjects;
using BadScript.Utility.Serialization;

namespace BadScript.Console.Subsystems.Project.BuildFormats
{

    public class BinaryOutputFormat : BuildOutputFormat
    {

        #region Public

        public BinaryOutputFormat() : base( "bin", "cbs" )
        {
        }

        public override void BuildOutput( BuildTarget t, string src, string outputFile )
        {
            using ( Stream str = File.Open( outputFile, FileMode.CreateNew ) )
            {
                BSSerializer.Serialize( new BSParser( src ).ParseToEnd(), str );
            }
        }

        #endregion

    }

}

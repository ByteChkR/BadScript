using System.IO;

using BadScript.Console.Preprocessor;

namespace BadScript.Testing
{

    public static class Program
    {

        #region Public

        public static void Main( string[] args )
        {
            string file = "D:\\Users\\Tim\\Documents\\BadScript\\src\\BadScript.Testing\\PreprocessorTest.bs";

            System.Console.WriteLine( SourcePreprocessor.Preprocess( File.ReadAllText( file ), "DEBUG=true" ) );
        }

        #endregion

    }

}

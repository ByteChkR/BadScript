using System.IO;

using BadScript.Console.Subsystems.Project;
using BadScript.Utils;

namespace BadScript.Console.Preprocessor
{

    public class SourcePreprocessorDirectories : SimpleSingleton <SourcePreprocessorDirectories>
    {

        public string PreprocessorDirectory => Path.Combine(BSConsoleDirectories.Instance.DataDirectory, "preprocessor");
        public string PreprocessorIncludeDirectory =>
            Path.Combine(PreprocessorDirectory, "include");
        public string PreprocessorDirectiveDirectory =>
            Path.Combine(PreprocessorDirectory, "directives");

        public SourcePreprocessorDirectories()
        {
            Directory.CreateDirectory(PreprocessorDirectory);
            Directory.CreateDirectory(PreprocessorIncludeDirectory);
            Directory.CreateDirectory(PreprocessorDirectiveDirectory);
        }

    }

}

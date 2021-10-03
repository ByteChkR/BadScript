using System.IO;

using BadScript.Settings;

namespace BadScript.Console.Subsystems.Project
{

    public class ProjectSystemDirectories : SimpleSingleton < ProjectSystemDirectories >
    {

        public string ProjectSystemDirectory => Path.Combine( BSConsoleDirectories.Instance.DataDirectory, "project" );

        public string TemplateDirectory => Path.Combine( ProjectSystemDirectory, "templates" );

        public string PreprocessorDirectory => Path.Combine( ProjectSystemDirectory, "preprocessor" );

        public string PreprocessorIncludeDirectory => Path.Combine( PreprocessorDirectory, "include" );

        public string PreprocessorDirectiveDirectory => Path.Combine( PreprocessorDirectory, "directives" );

        #region Public

        public ProjectSystemDirectories()
        {
            Directory.CreateDirectory( ProjectSystemDirectory );
            Directory.CreateDirectory( TemplateDirectory );
            Directory.CreateDirectory( PreprocessorDirectory );
            Directory.CreateDirectory( PreprocessorIncludeDirectory );
            Directory.CreateDirectory( PreprocessorDirectiveDirectory );
        }

        #endregion

    }

}

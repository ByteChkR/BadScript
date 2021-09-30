using System.IO;

using BadScript.Console.Logging;
using BadScript.Console.Subsystems.Project.DataObjects;

namespace BadScript.Console.Subsystems.Project
{

    public static class ProjectCreator
    {


        #region Public

        public static int Create( ProjectCreatorSettings settings )
        {
            string templateDir = GetTemplate( settings.TemplateName );

            if ( !Directory.Exists( templateDir ) )
            {
                ConsoleWriter.ErrorLine( "Can not find Template: " + settings.TemplateName );

                return -1;
            }

            if ( Directory.GetFileSystemEntries( Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories ).
                           Length !=
                 0 )
            {
                ConsoleWriter.ErrorLine( "Can not create Project in a directory that is not empty." );
            }

            CopyTemplateFiles( templateDir, Directory.GetCurrentDirectory() );

            ModifyTemplate( settings.ProjectName, Directory.GetCurrentDirectory() );

            ConsoleWriter.SuccessLine( "Command 'project new' finished!" );

            return 0;
        }

        #endregion

        #region Private

        private static void CopyTemplateFiles( string templateDir, string targetDir )
        {
            ConsoleWriter.LogLine( "Copying Template Files..." );

            foreach ( string dirPath in Directory.GetDirectories(
                                                                 templateDir,
                                                                 "*",
                                                                 SearchOption.AllDirectories
                                                                ) )
            {
                Directory.CreateDirectory( dirPath.Replace( templateDir, targetDir ) );
            }

            foreach ( string newPath in Directory.GetFiles(
                                                           templateDir,
                                                           "*",
                                                           SearchOption.AllDirectories
                                                          ) )
            {
                File.Copy( newPath, newPath.Replace( templateDir, targetDir ) );
            }
        }

        private static string GetTemplate( string template )
        {
            string dir = Path.Combine(ProjectSystemDirectories.Instance.TemplateDirectory, template );

            return dir;
        }

        private static void ModifyTemplate( string name, string directory )
        {
            string p = Path.Combine( directory, "build-settings.json" );
            ConsoleWriter.LogLine( "Modifying Template..." );
            ProjectSettings template = ProjectSettings.Deserialize( File.ReadAllText( p ) );
            template.AppInfo = new AppInfo( name );
            File.WriteAllText( p, template.Serialize() );
        }

        #endregion

    }

}

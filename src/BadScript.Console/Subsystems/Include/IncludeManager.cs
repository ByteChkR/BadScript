using System.IO;

using BadScript.Console.Logging;

namespace BadScript.Console.Subsystems.Include
{

    public static class IncludeManager
    {

        #region Public

        public static int Process( IncludeManagerSettings settings )
        {
            if ( !Directory.Exists( EngineBuilderDirectories.Instance.IncludeDirectory ) )
            {
                Directory.CreateDirectory( EngineBuilderDirectories.Instance.IncludeDirectory );
            }

            int ret = 0;

            switch ( settings.Operation )
            {
                case IncludeManagerOperations.Add:
                    ret = AddInclude( settings.Target );

                    break;

                case IncludeManagerOperations.Remove:
                    ret = RemoveInclude( settings.Target );

                    break;

                case IncludeManagerOperations.List:
                    ListIncludedFiles();

                    break;

                default:
                    ConsoleWriter.ErrorLine( $"Unknown Operation: {settings.Operation}" );

                    return -1;
            }

            if ( ret != -1 )
            {
                if (!settings.NoLogo)
                    ConsoleWriter.SuccessLine( "Command 'include' finished!" );
            }

            return ret;
        }

        #endregion

        #region Private

        private static int AddInclude( string target )
        {
            if ( !File.Exists( target ) )
            {
                ConsoleWriter.ErrorLine( $"Can not find File: {target}" );

                return -1;
            }

            ConsoleWriter.LogLine( "Adding File: " + Path.GetFileName( target ) );
            string dst = Path.Combine( EngineBuilderDirectories.Instance.IncludeDirectory, Path.GetFileName( target ) );
            File.Copy( target, dst, true );

            return 0;
        }

        private static void ListIncludedFiles()
        {
            string[] files = Directory.GetFiles(
                                                EngineBuilderDirectories.Instance.IncludeDirectory,
                                                "*",
                                                SearchOption.AllDirectories
                                               );

            ConsoleWriter.LogLine( "Included Files: " );

            foreach ( string file in files )
            {
                ConsoleWriter.LogLine(
                                      $"\t- {file.Remove( 0, EngineBuilderDirectories.Instance.IncludeDirectory.Length )}"
                                     );
            }

            ConsoleWriter.LogLine( "" );
        }

        private static int RemoveInclude( string target )
        {
            string file = Path.Combine( EngineBuilderDirectories.Instance.IncludeDirectory, target );

            if ( !File.Exists( file ) )
            {
                ConsoleWriter.ErrorLine( $"Can not find File: {target}" );

                return -1;
            }

            ConsoleWriter.LogLine( "Removing File: " + Path.GetFileName( target ) );
            File.Delete( file );

            return 0;
        }

        #endregion

    }

}

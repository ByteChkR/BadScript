using System.IO;
using System.IO.Compression;
using System.Linq;

using BadScript.Console.Logging;

using Newtonsoft.Json;

namespace BadScript.Console.AppPackage
{

    public static class AppBuilder
    {

        #region Public

        public static int Build( AppBuilderSettings settings )
        {
            BSAppPackageManifest manifest = new BSAppPackageManifest
                                            {
                                                AppName = settings.AppName,
                                                AppVersion = settings.AppVersion,
                                                ExecutablePath = "src/main.bs",
                                                RequiredInterfaces =
                                                    settings.RequiredInterfaces.ToArray(),
                                                ResourcePath = "res/",
                                                RuntimeMinVersion = settings.RuntimeMin,
                                                RuntimeMaxVersion = settings.RuntimeMax
                                            };

            string tempDir = Path.Combine( Path.GetTempPath(), "build_temp" );

            if ( Directory.Exists( tempDir ) )
            {
                Directory.Delete( tempDir, true );
            }

            Directory.CreateDirectory( tempDir );

            Directory.CreateDirectory( Path.Combine( tempDir, "src" ) );
            Directory.CreateDirectory( Path.Combine( tempDir, "res" ) );
            ConsoleWriter.LogLine( "Copying Source File..." );
            File.Copy( settings.SourceFile, Path.Combine( tempDir, "src", "main.bs" ) );

            if ( Directory.Exists( settings.ResourcePath ) )
            {
                CopyResources( settings.ResourcePath, Path.Combine( tempDir, "res" ) );
            }

            ConsoleWriter.LogLine( "Writing Manifest..." );
            File.WriteAllText( Path.Combine( tempDir, "manifest.json" ), JsonConvert.SerializeObject( manifest ) );

            string outFile = Path.GetFullPath( settings.OutputFile );
            string outDir = Path.GetDirectoryName( outFile );

            if ( !Directory.Exists( outDir ) )
            {
                Directory.CreateDirectory( outDir );
            }

            ConsoleWriter.LogLine( "Creating Package..." );

            if ( File.Exists( outFile ) )
            {
                File.Delete( outFile );
            }

            ZipFile.CreateFromDirectory( tempDir, settings.OutputFile );

            ConsoleWriter.SuccessLine( "Done!" );

            return 0;
        }

        #endregion

        #region Private

        private static void CopyResources( string resourceDir, string targetDir )
        {
            ConsoleWriter.LogLine( "Copying Resource Files..." );

            foreach ( string dirPath in Directory.GetDirectories(
                                                                 resourceDir,
                                                                 "*",
                                                                 SearchOption.AllDirectories
                                                                ) )
            {
                Directory.CreateDirectory( dirPath.Replace( resourceDir, targetDir ) );
            }

            foreach ( string newPath in Directory.GetFiles(
                                                           resourceDir,
                                                           "*",
                                                           SearchOption.AllDirectories
                                                          ) )
            {
                File.Copy( newPath, newPath.Replace( resourceDir, targetDir ) );
            }
        }

        #endregion

    }

}

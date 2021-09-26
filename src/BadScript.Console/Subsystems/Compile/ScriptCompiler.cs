using System.IO;

using BadScript.Utility.Serialization;

namespace BadScript.Console.Subsystems.Compile
{

    public static class ScriptCompiler
    {

        #region Public

        public static int Compile( ScriptCompilerSettings settings )
        {
            if ( !File.Exists( settings.Input ) )
            {
                ConsoleWriter.ErrorLine( "Unable to find File: " + settings.Input );
            }

            if ( File.Exists( settings.Output ) )
            {
                ConsoleWriter.WarnLine( "Compilation will Overwrite File: " + settings.Output );
            }

            BSEngine engine = settings.CreateEngineSettings().Build();
            ConsoleWriter.LogLine( "Compiling..." );

            using ( Stream s = File.Open( settings.Output, FileMode.CreateNew, FileAccess.ReadWrite ) )
            {
                BSSerializer.Serialize( engine.ParseFile( settings.Input ), s );
            }

            ConsoleWriter.SuccessLine( "Command 'compile' finished!" );
            return 0;
        }

        #endregion

    }

}

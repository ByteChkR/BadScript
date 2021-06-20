using System;
using System.IO.Compression;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Zip
{

    public static class ZipApi
    {

        #region Public

        public static void AddApi()
        {
            BSRuntimeTable t = new EngineRuntimeTable();

            t.InsertElement(
                            new EngineRuntimeObject( "createFromDirectory" ),
                            new BSRuntimeFunction(
                                                  "createFromDirectory(sourceDir, destinationFile)",
                                                  CreateFromFolder
                                                 )
                           );

            t.InsertElement(
                            new EngineRuntimeObject( "extractToDirectory" ),
                            new BSRuntimeFunction(
                                                  "extractToDirectory(sourceFile, destinationDir)",
                                                  ExtractToFolder
                                                 )
                           );

            BSEngine.AddStatic( "zipfile", t );
        }

        #endregion

        #region Private

        private static BSRuntimeObject CreateFromFolder( BSRuntimeObject[] args )
        {
            if ( args[0].TryConvertString( out string source ) &&
                 args[1].TryConvertString( out string destinationFile ) )
            {
                ZipFile.CreateFromDirectory( source, destinationFile );

                return new EngineRuntimeObject( null );
            }

            throw new Exception( "Expected string" );
        }

        private static BSRuntimeObject ExtractToFolder( BSRuntimeObject[] args )
        {
            if ( args[0].TryConvertString( out string sourceFile ) &&
                 args[1].TryConvertString( out string destination ) )
            {
                ZipFile.ExtractToDirectory( sourceFile, destination );

                return new EngineRuntimeObject( null );
            }

            throw new Exception( "Expected string" );
        }

        #endregion

    }

}

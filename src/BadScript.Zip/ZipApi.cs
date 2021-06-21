using System;
using System.IO.Compression;

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Zip
{

    public static class ZipApi
    {

        #region Public

        public static void AddApi()
        {
            ABSTable t = new BSTable();

            t.InsertElement(
                            new BSObject( "createFromDirectory" ),
                            new BSFunction(
                                                  "createFromDirectory(sourceDir, destinationFile)",
                                                  CreateFromFolder
                                                 )
                           );

            t.InsertElement(
                            new BSObject( "extractToDirectory" ),
                            new BSFunction(
                                                  "extractToDirectory(sourceFile, destinationDir)",
                                                  ExtractToFolder
                                                 )
                           );

            BSEngine.AddStatic( "zipfile", t );
        }

        #endregion

        #region Private

        private static ABSObject CreateFromFolder( ABSObject[] args )
        {
            if ( args[0].TryConvertString( out string source ) &&
                 args[1].TryConvertString( out string destinationFile ) )
            {
                ZipFile.CreateFromDirectory( source, destinationFile );

                return new BSObject( null );
            }

            throw new Exception( "Expected string" );
        }

        private static ABSObject ExtractToFolder( ABSObject[] args )
        {
            if ( args[0].TryConvertString( out string sourceFile ) &&
                 args[1].TryConvertString( out string destination ) )
            {
                ZipFile.ExtractToDirectory( sourceFile, destination );

                return new BSObject( null );
            }

            throw new Exception( "Expected string" );
        }

        #endregion

    }

}

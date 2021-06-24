using System.IO.Compression;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

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
                    CreateFromFolder,
                    2
                )
            );

            t.InsertElement(
                new BSObject( "extractToDirectory" ),
                new BSFunction(
                    "extractToDirectory(sourceFile, destinationDir)",
                    ExtractToFolder,
                    2
                )
            );

            BSEngine.AddStatic( "zipfile", t );
        }

        #endregion

        #region Private

        private static ABSObject CreateFromFolder( ABSObject[] args )
        {
            ZipFile.CreateFromDirectory(
                args[0].ResolveReference().ConvertString(),
                args[1].ResolveReference().ConvertString() );

            return new BSObject( null );
        }

        private static ABSObject ExtractToFolder( ABSObject[] args )
        {
            ZipFile.ExtractToDirectory(
                args[0].ResolveReference().ConvertString(),
                args[1].ResolveReference().ConvertString() );

            return new BSObject( null );
        }

        #endregion
    }

}

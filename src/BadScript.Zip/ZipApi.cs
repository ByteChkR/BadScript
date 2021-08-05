using System.IO.Compression;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Interfaces;

namespace BadScript.Zip
{

    public class ZipApi : ABSScriptInterface
    {
        #region Public

        public ZipApi() : base( "zip" )
        {
        }

        public override void AddApi( ABSTable t )
        {
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
        }

        #endregion

        #region Private

        private static ABSObject CreateFromFolder( ABSObject[] args )
        {
            ZipFile.CreateFromDirectory(
                args[0].ResolveReference().ConvertString(),
                args[1].ResolveReference().ConvertString() );

            return BSObject.Null;
        }

        private static ABSObject ExtractToFolder( ABSObject[] args )
        {
            ZipFile.ExtractToDirectory(
                args[0].ResolveReference().ConvertString(),
                args[1].ResolveReference().ConvertString() );

            return BSObject.Null;
        }

        #endregion
    }

}

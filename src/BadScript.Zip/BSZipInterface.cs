using System.IO.Compression;

using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Zip
{
    public class BSZipInterface : ABSScriptInterface
    {

        #region Public

        public BSZipInterface() : base( "Zip" )
        {
        }

        public override void AddApi( ABSTable t )
        {
            t.InsertElement(
                            new BSObject( "CreateFromDirectory" ),
                            new BSFunction(
                                           "CreateFromDirectory(sourceDir, destinationFile)",
                                           CreateFromFolder,
                                           2
                                          )
                           );

            t.InsertElement(
                            new BSObject( "ExtractToDirectory" ),
                            new BSFunction(
                                           "ExtractToDirectory(sourceFile, destinationDir)",
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
                                        args[1].ResolveReference().ConvertString()
                                       );

            return BSObject.Null;
        }

        private static ABSObject ExtractToFolder( ABSObject[] args )
        {
            ZipFile.ExtractToDirectory(
                                       args[0].ResolveReference().ConvertString(),
                                       args[1].ResolveReference().ConvertString()
                                      );

            return BSObject.Null;
        }

        #endregion

    }

}

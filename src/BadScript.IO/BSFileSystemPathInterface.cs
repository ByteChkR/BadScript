using System.IO;
using System.Linq;

using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.IO
{

    public class BSFileSystemPathInterface : ABSScriptInterface
    {

        private readonly string m_RootPath;

        #region Public

        public BSFileSystemPathInterface( string rootPath ) : base( "path" )
        {
            m_RootPath = Path.GetFullPath( rootPath );
        }

        public override void AddApi( ABSTable root )
        {
            GeneratePathApi( root, m_RootPath );
        }

        #endregion

        #region Private

        private static void GeneratePathApi( ABSTable ret, string root )
        {
            ret.InsertElement(
                              new BSObject( "getAppPath" ),
                              new BSFunction(
                                             "function getAppPath()",
                                             args => { return new BSObject( Path.GetFullPath( root ) ); },
                                             0
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getFullPath" ),
                              new BSFunction(
                                             "function getFullPath(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject( Path.GetFullPath( o.ConvertString() ) );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getDirectoryName" ),
                              new BSFunction(
                                             "function getDirectoryName(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject(
                                                                     Path.GetDirectoryName( o.ConvertString() )
                                                                    );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getFileName" ),
                              new BSFunction(
                                             "function getFileName(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject( Path.GetFileName( o.ConvertString() ) );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getFileNameWithoutExtension" ),
                              new BSFunction(
                                             "function getFileNameWithoutExtension(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject(
                                                                     Path.GetFileNameWithoutExtension(
                                                                          o.ConvertString()
                                                                         )
                                                                    );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getExtension" ),
                              new BSFunction(
                                             "function getExtension(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject( Path.GetExtension( o.ConvertString() ) );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getRandomFileName" ),
                              new BSFunction(
                                             "function getRandomFileName()",
                                             args => new BSObject( Path.GetRandomFileName() ),
                                             0
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getTempFileName" ),
                              new BSFunction(
                                             "function getTempFileName()",
                                             args => new BSObject( Path.GetTempFileName() ),
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "getTempPath" ),
                              new BSFunction(
                                             "function getTempPath()",
                                             args => new BSObject( Path.GetTempPath() ),
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "hasExtension" ),
                              new BSFunction(
                                             "function hasExtension(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject(
                                                                     ( decimal )( Path.HasExtension( o.ConvertString() )
                                                                                 ? 1
                                                                                 : 0 )
                                                                    );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "combine" ),
                              new BSFunction(
                                             "function combine(path0, path1, ...)",
                                             args =>
                                             {
                                                 return new BSObject(
                                                                     Path.Combine(
                                                                          args.Select(
                                                                                    x => BSReferenceExtensions.
                                                                                        ResolveReference( x ).
                                                                                        ConvertString()
                                                                                   ).
                                                                               ToArray()
                                                                         )
                                                                    );
                                             },
                                             1,
                                             int.MaxValue
                                            )
                             );
        }

        #endregion

    }

}

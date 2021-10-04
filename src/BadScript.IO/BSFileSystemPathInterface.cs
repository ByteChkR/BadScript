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

        public BSFileSystemPathInterface( string rootPath ) : base( "Path" )
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
                              new BSObject( "GetAppPath" ),
                              new BSFunction(
                                             "function GetAppPath()",
                                             args => { return new BSObject( Path.GetFullPath( root ) ); },
                                             0
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetFullPath" ),
                              new BSFunction(
                                             "function GetFullPath(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject( Path.GetFullPath( o.ConvertString() ) );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetDirectoryName" ),
                              new BSFunction(
                                             "function GetDirectoryName(path)",
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
                              new BSObject( "GetFileName" ),
                              new BSFunction(
                                             "function GetFileName(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject( Path.GetFileName( o.ConvertString() ) );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetFileNameWithoutExtension" ),
                              new BSFunction(
                                             "function GetFileNameWithoutExtension(path)",
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
                              new BSObject( "GetExtension" ),
                              new BSFunction(
                                             "function GetExtension(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return new BSObject( Path.GetExtension( o.ConvertString() ) );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetRandomFileName" ),
                              new BSFunction(
                                             "function GetRandomFileName()",
                                             args => new BSObject( Path.GetRandomFileName() ),
                                             0
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetTempFileName" ),
                              new BSFunction(
                                             "function GetTempFileName()",
                                             args => new BSObject( Path.GetTempFileName() ),
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetTempPath" ),
                              new BSFunction(
                                             "function GetTempPath()",
                                             args => new BSObject( Path.GetTempPath() ),
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "HasExtension" ),
                              new BSFunction(
                                             "function HasExtension(path)",
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
                              new BSObject( "Combine" ),
                              new BSFunction(
                                             "function Combine(path0, path1, ...)",
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

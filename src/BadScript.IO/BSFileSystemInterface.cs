using System.IO;
using System.Linq;

using BadScript.Interfaces;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.IO
{

    public class BSFileSystemInterface : ABSScriptInterface
    {

        #region Public

        public BSFileSystemInterface() : base( "FileSystem" )
        {
        }

        public override void AddApi( ABSTable root )
        {
            GenerateFSApi( root );
        }

        #endregion

        #region Private

        private static void CopyDir( string src, string dst )
        {
            foreach ( string dirPath in Directory.GetDirectories(
                                                                 src,
                                                                 "*",
                                                                 SearchOption.AllDirectories
                                                                ) )
            {
                Directory.CreateDirectory( dirPath.Replace( src, dst ) );
            }

            foreach ( string newPath in Directory.GetFiles(
                                                           src,
                                                           "*.*",
                                                           SearchOption.AllDirectories
                                                          ) )
            {
                File.Copy( newPath, newPath.Replace( src, dst ), true );
            }
        }

        private static void GenerateFSApi( ABSTable ret )
        {
            ret.InsertElement(
                              new BSObject( "Exists" ),
                              new BSFunction(
                                             "function Exists(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 string path = o.ConvertString();

                                                 return File.Exists( path ) ||
                                                        Directory.Exists( path )
                                                            ? BSObject.True
                                                            : BSObject.False;
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "SetCurrentDir" ),
                              new BSFunction(
                                             "function SetCurrentDir(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();
                                                 string path = o.ConvertString();

                                                 Directory.SetCurrentDirectory( path );

                                                 return BSObject.Null;
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetFiles" ),
                              new BSFunction(
                                             "function GetFiles(path)/GetFiles(path, searchPattern)/GetFiles(path, searchPattern, recurse)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 string path = o.ConvertString();

                                                 if ( args.Length >= 1 )
                                                 {
                                                     string pattern = args[1].ResolveReference().ConvertString();

                                                     bool recurse = false;

                                                     if ( args.Length > 2 )
                                                     {
                                                         recurse = args[2].ResolveReference().ConvertBool();
                                                     }

                                                     return new BSArray(
                                                                        Directory.GetFiles(
                                                                                 path,
                                                                                 pattern,
                                                                                 recurse
                                                                                     ? SearchOption.AllDirectories
                                                                                     : SearchOption.TopDirectoryOnly
                                                                                ).
                                                                            Select( x => new BSObject( x ) )
                                                                       );
                                                 }

                                                 return new BSArray(
                                                                    Directory.GetFiles( path ).
                                                                              Select( x => new BSObject( x ) )
                                                                   );
                                             },
                                             1,
                                             3
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetDirectories" ),
                              new BSFunction(
                                             "function GetDirectories(path)/GetDirectories(path, searchPattern)/GetDirectories(path, searchPattern, recurse)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 string path = o.ConvertString();

                                                 if ( args.Length >= 1 )
                                                 {
                                                     string pattern = args[1].ResolveReference().ConvertString();

                                                     bool recurse = false;

                                                     if ( args.Length > 2 )
                                                     {
                                                         recurse = args[2].ResolveReference().ConvertBool();
                                                     }

                                                     return new BSArray(
                                                                        Directory.GetDirectories(
                                                                                 path,
                                                                                 pattern,
                                                                                 recurse
                                                                                     ? SearchOption.AllDirectories
                                                                                     : SearchOption.TopDirectoryOnly
                                                                                ).
                                                                            Select( x => new BSObject( x ) )
                                                                       );
                                                 }

                                                 return new BSArray(
                                                                    Directory.GetDirectories( path ).
                                                                              Select( x => new BSObject( x ) )
                                                                   );
                                             },
                                             1,
                                             3
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "GetCurrentDir" ),
                              new BSFunction(
                                             "function GetCurrentDir()",
                                             args =>
                                             {
                                                 return new BSObject(
                                                                     Directory.GetCurrentDirectory()
                                                                    );
                                             },
                                             0
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "IsDir" ),
                              new BSFunction(
                                             "function IsDir(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return Directory.Exists( o.ConvertString() )
                                                            ? BSObject.True
                                                            : BSObject.False;
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "IsFile" ),
                              new BSFunction(
                                             "function IsFile(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 return File.Exists( o.ConvertString() )
                                                            ? BSObject.True
                                                            : BSObject.False;
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "CreateDir" ),
                              new BSFunction(
                                             "function CreateDir(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();
                                                 string source = o.ConvertString();
                                                 Directory.CreateDirectory( source );

                                                 return BSObject.Null;
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "Copy" ),
                              new BSFunction(
                                             "function Copy(source, destination)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 string source = o.ConvertString();
                                                 string destination = args[1].ResolveReference().ConvertString();

                                                 if ( Directory.Exists( source ) )
                                                 {
                                                     CopyDir( source, destination );
                                                 }
                                                 else
                                                 {
                                                     File.Copy( source, destination, true );
                                                 }

                                                 return BSObject.Null;
                                             },
                                             2
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "Move" ),
                              new BSFunction(
                                             "function Move(source, destination)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();
                                                 string source = o.ConvertString();
                                                 string destination = args[1].ResolveReference().ConvertString();

                                                 if ( Directory.Exists( source ) )
                                                 {
                                                     Directory.Move( source, destination );
                                                 }
                                                 else
                                                 {
                                                     File.Move( source, destination );
                                                 }

                                                 return BSObject.Null;
                                             },
                                             2
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "Delete" ),
                              new BSFunction(
                                             "function Delete(path)|function Delete(path, recurse)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();

                                                 string path = o.ConvertString();

                                                 if ( Directory.Exists( path ) )
                                                 {
                                                     bool recurse =
                                                         args.Length > 1 &&
                                                         args[1].ResolveReference().ConvertBool();

                                                     Directory.Delete( path, recurse );
                                                 }
                                                 else if ( File.Exists( path ) )
                                                 {
                                                     File.Delete( path );
                                                 }

                                                 return BSObject.Null;
                                             },
                                             1,
                                             2
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "Crc" ),
                              new BSFunction(
                                             "function Crc(path)",
                                             GetCheckSum,
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "Open" ),
                              new BSFunction(
                                             "function Open(path)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();
                                                 FileAccess m = FileAccess.ReadWrite;

                                                 return new BSStreamObject(
                                                                           o.Position,
                                                                           File.Open(
                                                                                o.ConvertString(),
                                                                                FileMode.OpenOrCreate,
                                                                                m
                                                                               )
                                                                          );
                                             },
                                             1
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "WriteAll" ),
                              new BSFunction(
                                             "function WriteAll(path, data)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();
                                                 ABSObject d = args[1].ResolveReference();
                                                 FileAccess m = FileAccess.ReadWrite;

                                                 File.WriteAllText( o.ConvertString(), d.ConvertString() );

                                                 return BSObject.Null;
                                             },
                                             2,
                                             2
                                            )
                             );

            ret.InsertElement(
                              new BSObject( "ReadAll" ),
                              new BSFunction(
                                             "function ReadAll(path, data)",
                                             args =>
                                             {
                                                 ABSObject o = args[0].ResolveReference();
                                                 FileAccess m = FileAccess.ReadWrite;

                                                 return new BSObject( File.ReadAllText( o.ConvertString() ) );
                                             },
                                             1,
                                             1
                                            )
                             );
        }

        private static ABSObject GetCheckSum( ABSObject[] arg )
        {
            return new BSObject( ( decimal )Crc.GetCrc( arg[0].ConvertString() ) );
        }

        #endregion

    }

}

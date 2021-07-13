using System.IO;
using System.Linq;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.IO
{

    public class BSFileSystemInterface : ABSScriptInterface
    {
        #region Public

        public BSFileSystemInterface() : base( "fs" )
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
                new BSObject( "exists" ),
                new BSFunction(
                    "function exists(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        string path = o.ConvertString();

                        return new BSObject(
                            ( decimal ) ( File.Exists( path ) ||
                                          Directory.Exists( path )
                                ? 1
                                : 0 )
                        );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "setCurrentDir" ),
                new BSFunction(
                    "function setCurrentDir(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();
                        string path = o.ConvertString();

                        Directory.SetCurrentDirectory( path );

                        return new BSObject( null );

                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "getFiles" ),
                new BSFunction(
                    "function getFiles(path)/getFiles(path, searchPattern)/getFiles(path, searchPattern, recurse)",
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
                        else
                        {
                            return new BSArray(
                                Directory.GetFiles( path ).
                                          Select( x => new BSObject( x ) )
                            );
                        }
                    },
                    1,
                    3
                )
            );

            ret.InsertElement(
                new BSObject( "getDirectories" ),
                new BSFunction(
                    "function getDirectories(path)/getDirectories(path, searchPattern)/getDirectories(path, searchPattern, recurse)",
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
                        else
                        {
                            return new BSArray(
                                Directory.GetDirectories( path ).
                                          Select( x => new BSObject( x ) )
                            );
                        }
                    },
                    1,
                    3
                )
            );

            ret.InsertElement(
                new BSObject( "getCurrentDir" ),
                new BSFunction(
                    "function getCurrentDir()",
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
                new BSObject( "isDir" ),
                new BSFunction(
                    "function isDir(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        return new BSObject(
                            ( decimal ) ( Directory.Exists( o.ConvertString() ) ? 1 : 0 )
                        );

                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "isFile" ),
                new BSFunction(
                    "function isFile(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        return new BSObject(
                            ( decimal ) ( File.Exists( o.ConvertString() ) ? 1 : 0 )
                        );
                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "createDir" ),
                new BSFunction(
                    "function createDir(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();
                        string source = o.ConvertString();
                        Directory.CreateDirectory( source );

                        return new BSObject( null );

                    },
                    1
                )
            );

            ret.InsertElement(
                new BSObject( "copy" ),
                new BSFunction(
                    "function copy(source, destination)",
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

                        return new BSObject( null );

                    },
                    2
                )
            );

            ret.InsertElement(
                new BSObject( "move" ),
                new BSFunction(
                    "function move(source, destination)",
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

                        return new BSObject( null );
                    },
                    2
                )
            );

            ret.InsertElement(
                new BSObject( "delete" ),
                new BSFunction(
                    "function delete(path)|function delete(path, recurse)",
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

                        return new BSObject( null );
                    },
                    1,
                    2
                )
            );

            ret.InsertElement(
                new BSObject( "open" ),
                new BSFunction(
                    "function open(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();
                        FileAccess m = FileAccess.ReadWrite;

                        return new BSFileSystemObject(
                            o.Position,
                            o.ConvertString(),
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
                new BSObject( "writeAll" ),
                new BSFunction(
                    "function writeAll(path, data)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();
                        ABSObject d = args[1].ResolveReference();
                        FileAccess m = FileAccess.ReadWrite;

                        File.WriteAllText( o.ConvertString(), d.ConvertString() );

                        return new BSObject( null );
                    },
                    2,
                    2
                )
            );

            ret.InsertElement(
                new BSObject( "readAll" ),
                new BSFunction(
                    "function readAll(path, data)",
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

        #endregion
    }

}

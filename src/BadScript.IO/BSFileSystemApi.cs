using System;
using System.IO;
using System.Linq;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.IO
{

    public static class BSFileSystemApi
    {
        #region Public

        public static void AddApi()
        {
            BSEngine.AddStatic( "fs", GenerateFSApi() );
            BSEngine.AddStatic( "path", GeneratePathApi() );
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

        private static ABSTable GenerateFSApi()
        {
            BSTable ret = new BSTable();

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
                    1
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

            return ret;
        }

        private static ABSTable GeneratePathApi()
        {
            BSTable ret = new BSTable();

            ret.InsertElement(
                new BSObject("getAppPath"),
                new BSFunction(
                    "function getAppPath()",
                    args =>
                    {
                        return new BSObject( Path.GetFullPath( AppDomain.CurrentDomain.BaseDirectory ) );

                    },
                    0
                )
            );


            ret.InsertElement(
                new BSObject("getFullPath"),
                new BSFunction(
                    "function getFullPath(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        return new BSObject(Path.GetFullPath(o.ConvertString()));

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
                            Path.GetFileNameWithoutExtension( o.ConvertString() )
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
                    0 )
            );

            ret.InsertElement(
                new BSObject( "getTempFileName" ),
                new BSFunction(
                    "function getTempFileName()",
                    args => new BSObject( Path.GetTempFileName() ),
                    1 )
            );

            ret.InsertElement(
                new BSObject( "getTempPath" ),
                new BSFunction(
                    "function getTempPath()",
                    args => new BSObject( Path.GetTempPath() ),
                    1 )
            );

            ret.InsertElement(
                new BSObject( "hasExtension" ),
                new BSFunction(
                    "function hasExtension(path)",
                    args =>
                    {
                        ABSObject o = args[0].ResolveReference();

                        return new BSObject(
                            ( decimal ) ( Path.HasExtension( o.ConvertString() ) ? 1 : 0 )
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
                        ABSObject o = args[0].ResolveReference();

                        return new BSObject(
                            Path.Combine(
                                args.Select( x => x.ResolveReference().ConvertString() ).
                                     ToArray()
                            )
                        );
                    },
                    1,
                    int.MaxValue
                )
            );

            ret.Lock();

            return ret;
        }

        #endregion
    }

}

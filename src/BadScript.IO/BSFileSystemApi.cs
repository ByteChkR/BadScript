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

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject(
                                                                 ( decimal ) ( File.Exists( path ) ||
                                                                               Directory.Exists( path )
                                                                                   ? 1
                                                                                   : 0 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "setCurrentDir" ),
                              new BSFunction(
                                                    "function setCurrentDir(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            Directory.SetCurrentDirectory( path );

                                                            return new BSObject( null );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getFiles" ),
                              new BSFunction(
                                                    "function getFiles(path)/getFiles(path, searchPattern)/getFiles(path, searchPattern, recurse)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( !o.TryConvertString( out string path ) )
                                                        {
                                                            throw new Exception( "Expected String" );
                                                        }

                                                        if ( args.Length >= 1 )
                                                        {
                                                            if ( !args[1].TryConvertString( out string pattern ) )
                                                            {
                                                                throw new Exception( "Expected String" );
                                                            }

                                                            bool recurse = false;

                                                            if ( args.Length >= 2 )
                                                            {
                                                                if ( args.Length > 3 )
                                                                {
                                                                    bool r = args[2].TryConvertBool( out bool rr );
                                                                    recurse = r && rr;
                                                                }
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

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getDirectories" ),
                              new BSFunction(
                                                    "function getDirectories(path)/getDirectories(path, searchPattern)/getDirectories(path, searchPattern, recurse)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( !o.TryConvertString( out string path ) )
                                                        {
                                                            throw new Exception( "Expected String" );
                                                        }

                                                        if ( args.Length >= 1 )
                                                        {
                                                            if ( !args[1].TryConvertString( out string pattern ) )
                                                            {
                                                                throw new Exception( "Expected String" );
                                                            }

                                                            bool recurse = false;

                                                            if ( args.Length >= 2 )
                                                            {
                                                                if ( args.Length > 3 )
                                                                {
                                                                    bool r = args[2].TryConvertBool( out bool rr );
                                                                    recurse = r && rr;
                                                                }
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
                                                    }
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
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "isDir" ),
                              new BSFunction(
                                                    "function isDir(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject(
                                                                 ( decimal ) ( Directory.Exists( path ) ? 1 : 0 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "isFile" ),
                              new BSFunction(
                                                    "function isFile(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject(
                                                                 ( decimal ) ( File.Exists( path ) ? 1 : 0 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "createDir" ),
                              new BSFunction(
                                                    "function createDir(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            Directory.CreateDirectory( path );

                                                            return new BSObject( null );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "copy" ),
                              new BSFunction(
                                                    "function copy(source, destination)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string source ) &&
                                                             args[1].TryConvertString( out string destination ) )

                                                        {
                                                            if ( Directory.Exists( source ) )
                                                            {
                                                                CopyDir( source, destination );
                                                            }
                                                            else
                                                            {
                                                                File.Copy( source, destination, true );
                                                            }

                                                            return new BSObject( null );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "move" ),
                              new BSFunction(
                                                    "function move(source, destination)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string source ) &&
                                                             args[1].TryConvertString( out string destination ) )

                                                        {
                                                            if ( Directory.Exists( source ) )
                                                            {
                                                                Directory.Move( source, destination );
                                                            }
                                                            else
                                                            {
                                                                File.Move( source, destination );
                                                            }

                                                            return new BSObject( null );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "delete" ),
                              new BSFunction(
                                                    "function delete(path)|function delete(path, recurse)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            if ( Directory.Exists( path ) )
                                                            {
                                                                bool recurse =
                                                                    args.Length > 1 &&
                                                                    args[1].TryConvertBool( out recurse );

                                                                Directory.Delete( path, recurse );
                                                            }
                                                            else if ( File.Exists( path ) )
                                                            {
                                                                File.Delete( path );
                                                            }

                                                            return new BSObject( null );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
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

                                                        if ( args.Length < 1 )
                                                        {
                                                            ABSObject
                                                                mode = args[1];

                                                            if ( mode.TryConvertString( out string fileOpenMode ) )
                                                            {
                                                                if ( fileOpenMode == "r" )
                                                                {
                                                                    m = FileAccess.Read;
                                                                }
                                                                else if ( fileOpenMode == "rw" )
                                                                {
                                                                    m = FileAccess.ReadWrite;
                                                                }
                                                                else if ( fileOpenMode == "w" )
                                                                {
                                                                    m = FileAccess.Write;
                                                                }
                                                                else
                                                                {
                                                                    throw new Exception( "Expected String" );
                                                                }
                                                            }
                                                            else
                                                            {
                                                                throw new Exception( "Expected String" );
                                                            }
                                                        }

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSFileSystemObject(
                                                                 path,
                                                                 File.Open(
                                                                           path,
                                                                           FileMode.OpenOrCreate,
                                                                           m
                                                                          )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            return ret;
        }

        private static ABSTable GeneratePathApi()
        {
            BSTable ret = new BSTable();

            ret.InsertElement(
                              new BSObject( "getFullPath" ),
                              new BSFunction(
                                                    "function getFullPath(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject( Path.GetFullPath( path ) );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getDirectoryName" ),
                              new BSFunction(
                                                    "function getDirectoryName(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject(
                                                                 Path.GetDirectoryName( path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getFileName" ),
                              new BSFunction(
                                                    "function getFileName(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject( Path.GetFileName( path ) );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getFileNameWithoutExtension" ),
                              new BSFunction(
                                                    "function getFileNameWithoutExtension(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject(
                                                                 Path.GetFileNameWithoutExtension( path )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getExtension" ),
                              new BSFunction(
                                                    "function getExtension(path)",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject( Path.GetExtension( path ) );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getRandomFileName" ),
                              new BSFunction(
                                                    "function getRandomFileName()",
                                                    args => new BSObject( Path.GetRandomFileName() )
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getTempFileName" ),
                              new BSFunction(
                                                    "function getTempFileName()",
                                                    args => new BSObject( Path.GetTempFileName() )
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getTempPath" ),
                              new BSFunction(
                                                    "function getTempPath()",
                                                    args => new BSObject( Path.GetTempPath() )
                                                   )
                             );

            ret.InsertElement(
                              new BSObject( "getTempPath" ),
                              new BSFunction(
                                                    "function getTempPath()",
                                                    args =>
                                                    {
                                                        ABSObject o = args[0].ResolveReference();

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new BSObject(
                                                                 ( decimal ) ( Path.HasExtension( path ) ? 1 : 0 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
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
                                                                          args.Select( x => x.ConvertString() ).
                                                                               ToArray()
                                                                         )
                                                            );
                                                    }
                                                   )
                             );

            return ret;
        }

        #endregion

    }

}

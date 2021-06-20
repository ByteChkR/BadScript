using System;
using System.IO;
using System.Linq;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Apis.FileSystem
{

    public static class BSFileSystemApi
    {

        #region Public

        public static void AddApi()
        {
            BSEngine.AddStatic("fs", GenerateFSApi());
            BSEngine.AddStatic("path", GeneratePathApi());
        }

        #endregion
        private static void CopyDir(string src, string dst)
        {
            foreach (string dirPath in Directory.GetDirectories(
                                                                src,
                                                                "*",
                                                                SearchOption.AllDirectories
                                                               ))
            {
                Directory.CreateDirectory(dirPath.Replace(src, dst));
            }
            foreach (string newPath in Directory.GetFiles(
                                                          src,
                                                          "*.*",
                                                          SearchOption.AllDirectories
                                                         ))
            {
                File.Copy(newPath, newPath.Replace(src, dst), true);
            }
        }
        #region Private

        private static BSRuntimeTable GeneratePathApi()
        {
            EngineRuntimeTable ret = new EngineRuntimeTable();
            ret.InsertElement(
                              new EngineRuntimeObject("getFullPath"),
                              new BSRuntimeFunction(
                                                    "function getFullPath(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if (o.TryConvertString(out string path))
                                                        {
                                                            return new EngineRuntimeObject(Path.GetFullPath(path));
                                                        }

                                                        throw new Exception("Expected String");
                                                    }
                                                   )
                             );
            ret.InsertElement(
                              new EngineRuntimeObject("getDirectoryName"),
                              new BSRuntimeFunction(
                                                    "function getDirectoryName(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if (o.TryConvertString(out string path))
                                                        {
                                                            return new EngineRuntimeObject(Path.GetDirectoryName(path));
                                                        }

                                                        throw new Exception("Expected String");
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject("getFileName"),
                              new BSRuntimeFunction(
                                                    "function getFileName(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if (o.TryConvertString(out string path))
                                                        {
                                                            return new EngineRuntimeObject(Path.GetFileName(path));
                                                        }

                                                        throw new Exception("Expected String");
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject("getFileNameWithoutExtension"),
                              new BSRuntimeFunction(
                                                    "function getFileNameWithoutExtension(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if (o.TryConvertString(out string path))
                                                        {
                                                            return new EngineRuntimeObject(Path.GetFileNameWithoutExtension(path));
                                                        }

                                                        throw new Exception("Expected String");
                                                    }
                                                   )
                             );
            ret.InsertElement(
                              new EngineRuntimeObject("getExtension"),
                              new BSRuntimeFunction(
                                                    "function getExtension(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if (o.TryConvertString(out string path))
                                                        {
                                                            return new EngineRuntimeObject(Path.GetExtension(path));
                                                        }

                                                        throw new Exception("Expected String");
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject("getRandomFileName"),
                              new BSRuntimeFunction(
                                                    "function getRandomFileName()",
                                                    args => new EngineRuntimeObject(Path.GetRandomFileName())
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "getTempFileName" ),
                              new BSRuntimeFunction(
                                                    "function getTempFileName()",
                                                    args => new EngineRuntimeObject( Path.GetTempFileName() )
                                                   )
                             );
            ret.InsertElement(
                              new EngineRuntimeObject("getTempPath"),
                              new BSRuntimeFunction(
                                                    "function getTempPath()",
                                                    args => new EngineRuntimeObject(Path.GetTempPath())
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "getTempPath" ),
                              new BSRuntimeFunction(
                                                    "function getTempPath()",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) ( Path.HasExtension( path ) ? 1 : 0 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject("combine"),
                              new BSRuntimeFunction(
                                                    "function combine(path0, path1, ...)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }

                                                        return new EngineRuntimeObject(
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

        private static BSRuntimeTable GenerateFSApi()
        {
            EngineRuntimeTable ret = new EngineRuntimeTable();

            ret.InsertElement(
                              new EngineRuntimeObject( "exists" ),
                              new BSRuntimeFunction(
                                                    "function exists(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new EngineRuntimeObject(
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
                              new EngineRuntimeObject( "setCurrentDir" ),
                              new BSRuntimeFunction(
                                                    "function setCurrentDir(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            Directory.SetCurrentDirectory( path );

                                                            return new EngineRuntimeObject( null );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject("getFiles"),
                              new BSRuntimeFunction(
                                                    "function getFiles(path)/getFiles(path, searchPattern)/getFiles(path, searchPattern, recurse)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }
                                                        if (!o.TryConvertString(out string path))
                                                        {
                                                            throw new Exception("Expected String");
                                                        }

                                                        if (args.Length >= 1)
                                                        {
                                                            if (!args[1].TryConvertString(out string pattern))
                                                            {
                                                                throw new Exception("Expected String");
                                                            }
                                                            bool recurse = false;

                                                            if (args.Length >= 2)
                                                            {
                                                                if (args.Length > 3)
                                                                {
                                                                    bool r = args[2].TryConvertBool(out bool rr);
                                                                    recurse = r && rr;
                                                                }
                                                            }

                                                            return new EngineRuntimeArray(
                                                                 Directory.GetFiles(
                                                                                path,
                                                                                pattern,
                                                                                recurse
                                                                                    ? SearchOption.AllDirectories
                                                                                    : SearchOption.TopDirectoryOnly
                                                                               ).
                                                                           Select(x => new EngineRuntimeObject(x))
                                                                );
                                                        }
                                                        else
                                                        {
                                                            return new EngineRuntimeArray(Directory.GetFiles(path).
                                                                Select(x => new EngineRuntimeObject(x)));
                                                        }


                                                        throw new Exception("Expected String");
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject("getDirectories"),
                              new BSRuntimeFunction(
                                                    "function getDirectories(path)/getDirectories(path, searchPattern)/getDirectories(path, searchPattern, recurse)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        while (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }
                                                        if (!o.TryConvertString(out string path))
                                                        {
                                                            throw new Exception("Expected String");
                                                        }

                                                        if (args.Length >= 1)
                                                        {
                                                            if (!args[1].TryConvertString(out string pattern))
                                                            {
                                                                throw new Exception("Expected String");
                                                            }
                                                            bool recurse = false;

                                                            if (args.Length >= 2)
                                                            {
                                                                if (args.Length > 3)
                                                                {
                                                                    bool r = args[2].TryConvertBool(out bool rr);
                                                                    recurse = r && rr;
                                                                }
                                                            }

                                                            return new EngineRuntimeArray(
                                                                 Directory.GetDirectories(
                                                                                path,
                                                                                pattern,
                                                                                recurse
                                                                                    ? SearchOption.AllDirectories
                                                                                    : SearchOption.TopDirectoryOnly
                                                                               ).
                                                                           Select(x => new EngineRuntimeObject(x))
                                                                );
                                                        }
                                                        else
                                                        {
                                                            return new EngineRuntimeArray(Directory.GetDirectories(path).
                                                                Select(x => new EngineRuntimeObject(x)));
                                                        }
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "getCurrentDir" ),
                              new BSRuntimeFunction(
                                                    "function getCurrentDir()",
                                                    args =>
                                                    {
                                                        return new EngineRuntimeObject(
                                                             Directory.GetCurrentDirectory()
                                                            );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "isDir" ),
                              new BSRuntimeFunction(
                                                    "function isDir(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) ( Directory.Exists( path ) ? 1 : 0 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "isFile" ),
                              new BSRuntimeFunction(
                                                    "function isFile(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if ( o.TryConvertString( out string path ) )
                                                        {
                                                            return new EngineRuntimeObject(
                                                                 ( decimal ) ( File.Exists( path ) ? 1 : 0 )
                                                                );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject("createDir"),
                              new BSRuntimeFunction(
                                                    "function createDir(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if (o is BSRuntimeReference r)
                                                        {
                                                            o = r.Get();
                                                        }

                                                        if (o.TryConvertString(out string path))
                                                        {
                                                            Directory.CreateDirectory(path);

                                                            return new EngineRuntimeObject(null);
                                                        }

                                                        throw new Exception("Expected String");
                                                    }
                                                   )
                             );
            ret.InsertElement(
                                                  new EngineRuntimeObject("copy"),
                                                  new BSRuntimeFunction(
                                                                        "function copy(source, destination)",
                                                                        args =>
                                                                        {
                                                                            BSRuntimeObject o = args[0];

                                                                            if (o is BSRuntimeReference r)
                                                                            {
                                                                                o = r.Get();
                                                                            }

                                                                            if (o.TryConvertString(out string source) &&
                                                                                    args[1].TryConvertString(out string destination))

                                                                            {
                                                                                if (Directory.Exists(source))
                                                                                    CopyDir(source, destination);
                                                                                else
                                                                                {
                                                                                    File.Copy(source, destination, true);
                                                                                }
                                                                                return new EngineRuntimeObject(null);
                                                                            }

                                                                            throw new Exception("Expected String");
                                                                        }
                                                                       )
                                                 );

            ret.InsertElement(
                                                  new EngineRuntimeObject("move"),
                                                  new BSRuntimeFunction(
                                                                        "function move(source, destination)",
                                                                        args =>
                                                                        {
                                                                            BSRuntimeObject o = args[0];

                                                                            if (o is BSRuntimeReference r)
                                                                            {
                                                                                o = r.Get();
                                                                            }

                                                                            if (o.TryConvertString(out string source) &&
                                                                                    args[1].TryConvertString(out string destination))

                                                                            {
                                                                                if (Directory.Exists(source))
                                                                                    Directory.Move(source, destination);
                                                                                else
                                                                                {
                                                                                    File.Move(source, destination);
                                                                                }
                                                                                return new EngineRuntimeObject(null);
                                                                            }

                                                                            throw new Exception("Expected String");
                                                                        }
                                                                       )
                                                 );

            ret.InsertElement(
                              new EngineRuntimeObject( "delete" ),
                              new BSRuntimeFunction(
                                                    "function delete(path)|function delete(path, recurse)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
                                                        }

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

                                                            return new EngineRuntimeObject( null );
                                                        }

                                                        throw new Exception( "Expected String" );
                                                    }
                                                   )
                             );

            ret.InsertElement(
                              new EngineRuntimeObject( "open" ),
                              new BSRuntimeFunction(
                                                    "function open(path)",
                                                    args =>
                                                    {
                                                        BSRuntimeObject o = args[0];
                                                        FileAccess m = FileAccess.ReadWrite;

                                                        if ( args.Length < 1 )
                                                        {
                                                            BSRuntimeObject
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

                                                        if ( o is BSRuntimeReference r )
                                                        {
                                                            o = r.Get();
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

        #endregion

    }

}

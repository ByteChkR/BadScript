using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript
{

    public class BSEngine
    {

        private static BSEngine s_Instance;

        private readonly Dictionary < string, BSRuntimeObject > m_StaticData =
            new Dictionary < string, BSRuntimeObject >();

        #region Public

        public static void AddStatic( string k, BSRuntimeObject o )
        {
            GetInstance().AddStaticData( k, o );
        }

        public static BSEngineInstance CreateEngineInstance()
        {
            return new BSEngineInstance(
                                        new Dictionary < string, BSRuntimeObject >(
                                             GetInstance().m_StaticData
                                            )
                                       );
        }

        public void AddStaticData( string k, BSRuntimeObject o )
        {
            m_StaticData[k] = o;
        }

        public object GetPluginInstance()
        {
            return this;
        }

        #endregion

        #region Private

        private BSEngine()
        {
            AddStaticData(
                          "size",
                          new BSRuntimeFunction(
                                                "function size(table/array)",
                                                ( args ) =>
                                                {
                                                    if ( args.Length != 1 )
                                                    {
                                                        throw new Exception( "Invalid Argument Count" );
                                                    }

                                                    BSRuntimeObject arg = args[0];

                                                    if ( arg is BSRuntimeArrayReference
                                                             arRef )
                                                    {
                                                        arg = arRef.Get();
                                                    }

                                                    if ( arg is BSRuntimeTableReference
                                                             tRef )
                                                    {
                                                        arg = tRef.Get();
                                                    }

                                                    if ( arg is BSRuntimeArray ar )
                                                    {
                                                        return new EngineRuntimeObject( ( decimal ) ar.GetLength() );
                                                    }

                                                    if ( arg is BSRuntimeTable t )
                                                    {
                                                        return new EngineRuntimeObject( ( decimal ) t.GetLength() );
                                                    }

                                                    throw new Exception(
                                                                        "Can not get Size of object: " + arg
                                                                       );
                                                }
                                               )
                         );

            AddStaticData("format",
                          new BSRuntimeFunction("function format(formatStr, arg0, arg1, ...)",
                                                args =>
                                                {
                                                    if (args.Length < 1)
                                                    {
                                                        throw new Exception("Invalid Argument Count");
                                                    }

                                                    BSRuntimeObject format = args[0];

                                                    while (format is BSRuntimeReference
                                                               arRef)
                                                    {
                                                        format = arRef.Get();
                                                    }

                                                    if ( format.TryConvertString( out string formatStr ) )
                                                    {
                                                        return new EngineRuntimeObject(string.Format(formatStr, args.Skip(1).Cast<object>().ToArray()));
                                                    }

                                                    throw new Exception( "Invalid Format string type" );

                                                }));
            AddStaticData(
                          "print",
                          new BSRuntimeFunction(
                                                "function print(obj)",
                                                (args) =>
                                                {
                                                    if (args.Length != 1)
                                                    {
                                                        throw new Exception("Invalid Argument Count");
                                                    }

                                                    BSRuntimeObject arg = args[0];

                                                    if (arg is BSRuntimeReference
                                                            arRef)
                                                    {
                                                        arg = arRef.Get();
                                                    }

                                                    Console.WriteLine(arg);

                                                    return new EngineRuntimeObject(null);
                                                }
                                               )
                         );

            AddStaticData(
                          "read",
                          new BSRuntimeFunction(
                                                "function read()",
                                                (args) =>
                                                {
                                                    if (args.Length != 0)
                                                    {
                                                        throw new Exception("Invalid Argument Count");
                                                    }

                                                    
                                                    return new EngineRuntimeObject(Console.ReadLine());
                                                }
                                               )
                         );

            AddStaticData(
                          "sleep",
                          new BSRuntimeFunction(
                                                "function sleep(ms)",
                                                ( args ) =>
                                                {
                                                    if ( args[0].TryConvertDecimal( out decimal lD ) )
                                                    {
                                                        Thread.Sleep( ( int ) lD );

                                                        return new EngineRuntimeObject( null );
                                                    }

                                                    throw new Exception( "Invalid sleep(millis) Usage" );
                                                }
                                               )
                         );

        }

        private static BSEngine GetInstance()
        {
            return s_Instance ?? ( s_Instance = new BSEngine() );
        }

        #endregion

    }

}

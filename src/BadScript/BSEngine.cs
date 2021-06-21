using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Common.Types.References.Implementations;

namespace BadScript
{

    public class BSEngine
    {

        private static BSEngine s_Instance;

        private readonly Dictionary < string, ABSObject > m_StaticData =
            new Dictionary < string, ABSObject >();

        #region Public

        public static void AddStatic( string k, ABSObject o )
        {
            GetInstance().AddStaticData( k, o );
        }

        public static BSEngineInstance CreateEngineInstance()
        {
            return new BSEngineInstance(
                                        new Dictionary < string, ABSObject >(
                                             GetInstance().m_StaticData
                                            )
                                       );
        }

        public void AddStaticData( string k, ABSObject o )
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
                          new BSFunction(
                                                "function size(table/array)",
                                                ( args ) =>
                                                {
                                                    if ( args.Length != 1 )
                                                    {
                                                        throw new Exception( "Invalid Argument Count" );
                                                    }

                                                    ABSObject arg = args[0];

                                                    if ( arg is BSArrayReference
                                                             arRef )
                                                    {
                                                        arg = arRef.Get();
                                                    }

                                                    if ( arg is BSTableReference
                                                             tRef )
                                                    {
                                                        arg = tRef.Get();
                                                    }

                                                    if ( arg is ABSArray ar )
                                                    {
                                                        return new BSObject( ( decimal ) ar.GetLength() );
                                                    }

                                                    if ( arg is ABSTable t )
                                                    {
                                                        return new BSObject( ( decimal ) t.GetLength() );
                                                    }

                                                    throw new Exception(
                                                                        "Can not get Size of object: " + arg
                                                                       );
                                                }
                                               )
                         );

            AddStaticData(
                          "format",
                          new BSFunction(
                                                "function format(formatStr, arg0, arg1, ...)",
                                                args =>
                                                {
                                                    if ( args.Length < 1 )
                                                    {
                                                        throw new Exception( "Invalid Argument Count" );
                                                    }

                                                    ABSObject format = args[0].ResolveReference();

                                                    if ( format.TryConvertString( out string formatStr ) )
                                                    {
                                                        return new BSObject(
                                                             string.Format(
                                                                           formatStr,
                                                                           args.Skip( 1 ).Cast < object >().ToArray()
                                                                          )
                                                            );
                                                    }

                                                    throw new Exception( "Invalid Format string type" );
                                                }
                                               )
                         );

            AddStaticData(
                          "print",
                          new BSFunction(
                                                "function print(obj)",
                                                ( args ) =>
                                                {
                                                    if ( args.Length != 1 )
                                                    {
                                                        throw new Exception( "Invalid Argument Count" );
                                                    }

                                                    ABSObject arg = args[0].ResolveReference();

                                                    Console.WriteLine( arg );

                                                    return new BSObject( null );
                                                }
                                               )
                         );

            AddStaticData(
                          "read",
                          new BSFunction(
                                                "function read()",
                                                ( args ) =>
                                                {
                                                    if ( args.Length != 0 )
                                                    {
                                                        throw new Exception( "Invalid Argument Count" );
                                                    }

                                                    return new BSObject( Console.ReadLine() );
                                                }
                                               )
                         );

            AddStaticData(
                          "sleep",
                          new BSFunction(
                                                "function sleep(ms)",
                                                ( args ) =>
                                                {
                                                    if ( args[0].TryConvertDecimal( out decimal lD ) )
                                                    {
                                                        Thread.Sleep( ( int ) lD );

                                                        return new BSObject( null );
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

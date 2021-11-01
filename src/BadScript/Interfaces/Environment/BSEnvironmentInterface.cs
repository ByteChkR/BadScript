using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;

using BadScript.Exceptions;
using BadScript.Interfaces.Environment.Settings;
using BadScript.Parser.OperatorImplementations;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Interfaces.Environment
{

    public class BSEnvironmentInterface : ABSScriptInterface
    {

        private readonly BSEngine m_Instance;

        #region Public

        public BSEnvironmentInterface( BSEngine instance ) : base( "Environment" )
        {
            m_Instance = instance;
        }

        public override void AddApi( ABSTable env )
        {
            env.InsertElement(
                              new BSObject( "DefaultOp" ),
                              new BSFunction(
                                             "function DefaultOp(opKey, args..)",
                                             ExecuteDefaultOperator,
                                             1,
                                             int.MaxValue
                                            )
                             );

            env.InsertElement(
                              new BSObject( "CreateScope" ),
                              new BSFunction(
                                             "function CreateScope()/CreateScope(parentScope)",
                                             m_Instance.CreateScope,
                                             0,
                                             1
                                            )
                             );

            env.InsertElement(
                              new BSObject( "ResetScope" ),
                              new BSFunction(
                                             "function ResetScope(scope)",
                                             m_Instance.ResetScope,
                                             1
                                            )
                             );

            env.InsertElement(
                              new BSObject( "LoadScopedString" ),
                              new BSFunction(
                                             "function LoadScopedString(scope, str, args..)/LoadScopedString(rootTable, str, args..)",
                                             m_Instance.LoadStringScopedApi,
                                             2,
                                             int.MaxValue
                                            )
                             );

            env.InsertElement(
                              new BSObject( "LoadScopedBenchmark" ),
                              new BSFunction(
                                             "function LoadScopedBenchmark(scope, str, args..)/LoadScopedBenchmark(rootTable, str, args..)",
                                             m_Instance.LoadStringScopedBenchmarkApi,
                                             2,
                                             int.MaxValue
                                            )
                             );

            env.InsertElement(
                              new BSObject( "AddPreprocessor" ),
                              new BSFunction(
                                             "function AddPreprocessor(ppName, func)",
                                             m_Instance.AddPreprocessorApi,
                                             2
                                            )
                             );

            env.InsertElement(
                              new BSObject( "LoadString" ),
                              new BSFunction( "function LoadString(str)", m_Instance.LoadStringApi, 1, int.MaxValue )
                             );

            env.InsertElement(
                              new BSObject( "LoadBenchmark" ),
                              new BSFunction( "function LoadBenchmark(str)", m_Instance.LoadStringApi, 1, int.MaxValue )
                             );

            env.InsertElement(
                              new BSObject( "LoadInterface" ),
                              new BSFunction(
                                             "function LoadInterface(key)/LoadInterface(key, root)",
                                             m_Instance.LoadInterfaceApi,
                                             1,
                                             2
                                            )
                             );

            env.InsertElement(
                              new BSObject( "GetInterfaceNames" ),
                              new BSFunction( "function GetInterfaceNames()", m_Instance.GetInterfaceNamesApi, 0, 0 )
                             );

            env.InsertElement(
                              new BSObject( "HasInterface" ),
                              new BSFunction( "function HasInterface(interfaceName)", m_Instance.HasInterfaceName, 1 )
                             );

            env.InsertElement( new BSObject( "Settings" ), new SettingsCategoryWrapper( BSSettings.BsRoot ) );

            env.InsertElement(
                              new BSObject("Error"),
                              new BSFunction(
                                             "function Error(obj)",
                                             (args) =>
                                             {
                                                 ABSObject arg = args[0].ResolveReference();

                                                 throw new BSRuntimeException(arg.Position, arg.ToString());
                                             },
                                             1
                                            )
                             );
            env.InsertElement(
                              new BSObject("Throw"),
                              new BSFunction(
                                             "function Throw(message, exception)",
                                             (args) =>
                                             {
                                                 string msg = args[0].ResolveReference().ConvertString();
                                                 ABSObject exc = args[1].ResolveReference();

                                                 if (exc is IBSWrappedObject wo && wo.GetInternalObject() is Exception ex)
                                                 {
                                                     throw new BSRuntimeException(msg, ex);
                                                 }
                                                 throw new BSRuntimeException("Environment.Throw Expects an exception object as second argument.");
                                             },
                                             2
                                            )
                             );

            env.InsertElement(
                              new BSObject("Rethrow"),
                              new BSFunction(
                                             "function Rethrow(exception)",
                                             (args) =>
                                             {
                                                 ABSObject exc = args[0].ResolveReference();

                                                 if (exc is IBSWrappedObject wo && wo.GetInternalObject() is Exception ex)
                                                 {
                                                     ExceptionDispatchInfo.Capture( ex ).Throw();
                                                 }
                                                 throw new BSRuntimeException($"Environment.Throw Expects an exception object as second argument. {exc.SafeToString()}");
                                             },
                                             1
                                            )
                             );

            env.InsertElement(
                              new BSObject( "Sleep" ),
                              new BSFunction(
                                             "function Sleep(ms)",
                                             ( args ) =>
                                             {
                                                 if ( args[0].TryConvertDecimal( out decimal lD ) )
                                                 {
                                                     Thread.Sleep( ( int )lD );

                                                     return BSObject.Null;
                                                 }

                                                 throw new BSInvalidTypeException(
                                                      args[0].Position,
                                                      "Invalid Sleep Time",
                                                      args[0],
                                                      "number"
                                                     );
                                             },
                                             1
                                            )
                             );

            env.InsertElement(
                              new BSObject( "Debug" ),
                              new BSFunction(
                                             "function Debug(obj)",
                                             ( args ) =>
                                             {
                                                 ABSObject arg = args[0].ResolveReference();

                                                 return new BSObject(
                                                                     arg.SafeToString(
                                                                          new Dictionary < ABSObject, string >()
                                                                         )
                                                                    );
                                             },
                                             1
                                            )
                             );
        }

        #endregion

        #region Private

        private ABSObject ExecuteDefaultOperator( ABSObject[] arg )
        {
            ABSObject[] a = arg.Skip( 1 ).ToArray();

            ABSOperatorImplementation impl = BSOperatorImplementationResolver.ResolveImplementation(
                 arg[0].ConvertString(),
                 a,
                 false
                );

            return impl.ExecuteOperator( a );
        }

        #endregion

    }

}

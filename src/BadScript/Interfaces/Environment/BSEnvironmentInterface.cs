using System.Linq;

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
                              new BSObject("CreateScope"),
                              new BSFunction(
                                             "function CreateScope()/CreateScope(parentScope)",
                                             m_Instance.CreateScope,
                                             0,
                                             1
                                            )
                             );

            env.InsertElement(
                              new BSObject("ResetScope"),
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
                              new BSObject("LoadScopedBenchmark"),
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
                              new BSFunction("function LoadString(str)", m_Instance.LoadStringApi, 1, int.MaxValue )
                             );

            env.InsertElement(
                              new BSObject( "LoadBenchmark" ),
                              new BSFunction("function LoadBenchmark(str)", m_Instance.LoadStringApi, 1, int.MaxValue )
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
                              new BSObject( "IsLiteral" ),
                              new BSFunction(
                                             "function IsLiteral(v)",
                                             objects =>
                                             {
                                                 ABSObject o = objects[0].ResolveReference();

                                                 if ( o is BSObject bso )
                                                 {
                                                     return bso.IsLiteral ? BSObject.True : BSObject.False;
                                                 }

                                                 return BSObject.False;
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

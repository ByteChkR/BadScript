using System.Linq;
using BadScript.Common.OperatorImplementations;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Interfaces.Settings;
using BadScript.Settings;

namespace BadScript.Interfaces
{

    public class BSEngineEnvironmentInterface : ABSScriptInterface
    {
        private readonly BSEngine m_Instance;

        #region Public

        public BSEngineEnvironmentInterface( BSEngine instance ) : base( "environment" )
        {
            m_Instance = instance;
        }

        public override void AddApi( ABSTable env )
        {
            env.InsertElement(
                new BSObject( "defaultOp" ),
                new BSFunction( "function defaultOp(opKey, args..)", ExecuteDefaultOperator, 1, int.MaxValue ) );

            env.InsertElement(
                new BSObject( "createScope" ),
                new BSFunction( "function createScope()/createScope(parentScope)", m_Instance.CreateScope, 0, 1 ) );

            env.InsertElement(
                new BSObject( "loadScopedString" ),
                new BSFunction(
                    "function loadScopedString(scope, str, args..)/loadScopedString(rootTable, str, args..)",
                    m_Instance.LoadStringScopedApi,
                    2,
                    int.MaxValue ) );

            env.InsertElement(
                new BSObject( "loadScopedBenchmark" ),
                new BSFunction(
                    "function loadScopedBenchmark(scope, str, args..)/loadScopedBenchmark(rootTable, str, args..)",
                    m_Instance.LoadStringScopedBenchmarkApi,
                    2,
                    int.MaxValue ) );

            env.InsertElement(
                new BSObject( "addPreprocessor" ),
                new BSFunction( "function addPreprocessor(ppName, func)", m_Instance.AddPreprocessorApi, 2 )
            );

            env.InsertElement(
                new BSObject( "loadString" ),
                new BSFunction( "function loadString(str)", m_Instance.LoadStringApi, 1, int.MaxValue )
            );

            env.InsertElement(
                new BSObject( "loadBenchmark" ),
                new BSFunction( "function loadBenchmark(str)", m_Instance.LoadStringApi, 1, int.MaxValue )
            );

            env.InsertElement(
                new BSObject( "loadInterface" ),
                new BSFunction(
                    "function loadInterface(key)/loadInterface(key, root)",
                    m_Instance.LoadInterfaceApi,
                    1,
                    2 )
            );

            env.InsertElement(
                new BSObject( "getInterfaceNames" ),
                new BSFunction( "function getInterfaceNames()", m_Instance.GetInterfaceNamesApi, 0, 0 )
            );

            env.InsertElement(
                new BSObject( "hasInterface" ),
                new BSFunction( "function hasInterface(interfaceName)", m_Instance.HasInterfaceName, 1 )
            );

            env.InsertElement( new BSObject( "settings" ), new SettingsCategoryWrapper( BSSettings.BsRoot ) );
        }

        #endregion

        #region Private

        private ABSObject ExecuteDefaultOperator( ABSObject[] arg )
        {
            ABSObject[] a = arg.Skip( 1 ).ToArray();

            ABSOperatorImplementation impl = BSOperatorImplementationResolver.ResolveImplementation(
                arg[0].ConvertString(),
                a,
                false );

            return impl.ExecuteOperator( a );
        }

        #endregion
    }

}

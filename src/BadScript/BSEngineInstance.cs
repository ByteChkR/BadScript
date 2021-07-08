using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript
{

    public class BSEngineInstance
    {
        private readonly ABSTable m_StaticData;
        private readonly ABSTable m_GlobalTable;
        private readonly List < ABSScriptInterface > m_Interfaces;

        #region Public

        public BSEngineInstance(Dictionary<string, ABSObject> startObjects, List <ABSScriptInterface> interfaces, Dictionary<string, ABSObject> gTable = null)
        {
            m_Interfaces = interfaces;
            Dictionary<ABSObject, ABSObject> staticData =
                new Dictionary<ABSObject, ABSObject>();

            foreach (KeyValuePair<string, ABSObject> buildScriptEngineRuntimeObject in startObjects)
            {
                staticData[new BSObject(buildScriptEngineRuntimeObject.Key)] =
                    buildScriptEngineRuntimeObject.Value;
            }

            BSTable sd = new BSTable(staticData);

            
            sd.Lock();
            m_StaticData = sd;

            if (gTable == null)
            {
                m_GlobalTable = new BSTable();
            }
            else
            {
                Dictionary<ABSObject, ABSObject> globalTable =
                    new Dictionary<ABSObject, ABSObject>();
                foreach (KeyValuePair<string, ABSObject> buildScriptEngineRuntimeObject in gTable)
                {
                    globalTable[new BSObject(buildScriptEngineRuntimeObject.Key)] =
                        buildScriptEngineRuntimeObject.Value;
                }

                m_GlobalTable = new BSTable(globalTable);
            }

            BSTable env = new BSTable();

            env.InsertElement(
                new BSObject( "createScope" ),
                new BSFunction( "function createScope()", objects => CreateScope(), 0, 0 ) );
            env.InsertElement(
                new BSObject( "loadScopedString" ),
                new BSFunction(
                    "function loadScopedString(scope, str, args..)",
                    LoadStringScopedApi,
                    2,
                    int.MaxValue ) );
            env.InsertElement(
                new BSObject("loadString"),
                new BSFunction("function loadString(str)", LoadStringApi, 1, int.MaxValue)
            );
            env.InsertElement(
                new BSObject("loadInterface"),
                new BSFunction("function loadInterface(key)/loadInterface(key, root)", LoadInterfaceApi, 1, 2)
            );

            env.InsertElement(
                new BSObject( "getScriptBaseDir" ),
                new BSFunction( "function getScriptBaseDir()", GetScriptBaseDirApi, 0, 0 ) );
            env.InsertElement(
                new BSObject("getInterfaceNames"),
                new BSFunction("function getInterfaceNames()", GetInterfaceNamesApi, 0, 0)
            );

            env.Lock();
            
            m_GlobalTable.InsertElement(new BSObject("environment"), env);



            m_GlobalTable.InsertElement(new BSObject("__G"), m_GlobalTable);
        }

        private ABSObject GetScriptBaseDirApi( ABSObject[] arg )
        {
            return null;
        }

        private ABSObject GetInterfaceNamesApi( ABSObject[] arg )=> new BSArray( InterfaceNames.Select( x => new BSObject( x ) ) );
        
        public ABSTable GetGlobalTable() => m_GlobalTable;

        private ABSObject LoadInterfaceApi( ABSObject[] arg )
        {
            string key = arg[0].ConvertString();
            if(arg.Length == 2)
            {
                return LoadInterface( key, ( ABSTable ) arg[1] );
            }

           return LoadInterface(key);
        }

        internal bool HasElement(ABSObject name)
        {
            return m_GlobalTable.HasElement(name) || m_StaticData.HasElement(name);
        }

        internal void InsertElement( ABSObject k, ABSObject v ) => m_GlobalTable.InsertElement( k, v );

        internal ABSObject GetElement(ABSObject name)
        {
            if (m_GlobalTable.HasElement(name))
                return m_GlobalTable.GetElement(name);
            if (m_StaticData.HasElement(name))
                return m_StaticData.GetElement(name);

            throw new BSRuntimeException( $"Can not Resolve name: '{name.SafeToString()}'" );
        }

        public ABSObject LoadFile( string file, string[] args = null )
        {
            return LoadString( File.ReadAllText( file ), args );
        }

        public ABSObject LoadFile( string file, ABSObject[] args = null )
        {
            return LoadString( File.ReadAllText( file ), args );
        }

        public ABSObject LoadString( string script, string[] args = null )
        {
            return LoadString(
                script,
                args?.Select( x => ( ABSObject ) new BSObject( x ) ).ToArray()
            );
        }

        public ABSObject LoadString( BSScope scope, string script, string[] args = null )
        {
            return LoadString(
                scope,
                script,
                args?.Select(x => (ABSObject)new BSObject(x)).ToArray()
            );
        }
        public ABSObject LoadString(BSScope scope, string script, ABSObject[] args)
        {
            BSParser parser = new BSParser(script);
            BSExpression[] exprs = parser.ParseToEnd();


            scope.AddLocalVar(
                "args",
                args == null
                    ? (ABSObject)new BSObject(null)
                    : new BSArray(args)
            );

            foreach (BSExpression buildScriptExpression in exprs)
            {
                buildScriptExpression.Execute(scope);

                if (scope.BreakExecution)
                {
                    break;
                }
            }

            return scope.Return ?? scope.GetLocals();
        }

        public ABSObject LoadString( string script, ABSObject[] args = null )
        {
            return LoadString( new BSScope( this ), script, args );
        }



        #endregion

        #region Private

        private ABSObject CreateScope()
        {
            return new BSObject( new BSScope( this ) );
        }
        


        private ABSObject LoadStringScopedApi(ABSObject[] arg)
        {

            ABSObject scope = arg[0].ResolveReference();
            ABSObject o = arg[1].ResolveReference();

            if (o.TryConvertString(out string path))
            {
                if(scope is BSObject obj)
                {
                    BSScope sc = (BSScope)obj.GetInternalObject();
                    ABSObject ret = LoadString(sc, path, arg.Skip(2).ToArray());

                    return ret;
                }

                throw new BSRuntimeException( "'function loadScopedString(scope, script, args..)' requires a valid scope to be passed as first argument" );

            }

            throw new BSInvalidTypeException(
                "Expected String",
                o,
                "string"
            );
        }

        private ABSObject LoadStringApi( ABSObject[] arg )
        {
            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                ABSObject ret = LoadString( path, arg.Skip( 1 ).ToArray() );

                return ret;
            }

            throw new BSInvalidTypeException(
                "Expected String",
                o,
                "string"
            );
        }

        public void AddInterface( ABSScriptInterface i ) => m_Interfaces.Add( i );

        public string[] InterfaceNames => m_Interfaces.Select( x => x.Name ).ToArray();
        public bool HasInterface( string key ) => m_Interfaces.Any( x => x.Name == key );
        public ABSTable LoadInterface( string key, ABSTable t = null )
        {
            if ( !HasInterface( key ) )
            {
                throw new BSRuntimeException( $"Can not find interface: '{key}'" );
            }

            ABSTable MakeInterfaceTable()
            {
                ABSTable it = new BSTable();

                m_GlobalTable.InsertElement( new BSObject( key ), it );

                return it;
            }
            ABSTable table = t ?? MakeInterfaceTable();

            List < ABSScriptInterface> i = m_Interfaces.Where( x => x.Name == key ).ToList();
            i.ForEach(x=>x.AddApi( table ));

            return table;
        }

        #endregion
    }

}

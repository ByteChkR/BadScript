using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using BadScript.Common;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Interfaces;
using BadScript.Settings;
using BadScript.Utility.Optimization;
using BadScript.Utility.Serialization;
using BadScript.Utility.Validators;

namespace BadScript
{

    public class BSTypeDatabase
    {

        private readonly Dictionary<string, BSClassExpression> m_Classes =
            new Dictionary<string, BSClassExpression>();

        #region Public

        public void Clear()
        {
            m_Classes.Clear();
        }

        public BSClassInstance CreateInstance(string name, BSEngine engine, ABSObject[] args)
        {
            BSScope classScope = new BSScope(engine);

            return CreateBaseInstance(name, classScope, args);
        }

        internal void AddClass(BSClassExpression expr)
        {
            if (m_Classes.ContainsKey(expr.Name))
            {
                throw new BSRuntimeException(
                                             $"Can not Create Class Definition because a Type with the name '{expr.Name}' does already exist."
                                            );
            }

            m_Classes.Add(expr.Name, expr);
        }

        #endregion

        #region Private

        private BSClassInstance CreateBaseInstance(string name, BSScope scope, ABSObject[] args)
        {
            BSClassExpression expr = m_Classes[name];
            BSClassInstance baseInstance = null;

            if (expr.BaseName != null)
            {
                baseInstance = CreateBaseInstance(expr.BaseName, scope, null);
                scope = new BSScope(BSScopeFlags.None, baseInstance.InstanceScope);
            }

            m_Classes[name].AddClassData(scope);

            BSClassInstance table = new BSClassInstance(SourcePosition.Unknown, name, baseInstance, scope);

            if (args != null)
            {
                if (table.HasProperty(table.Name))
                {
                    ABSObject func = table.GetProperty(table.Name).ResolveReference();
                    func.Invoke(args);
                }
            }

            return table;
        }

        #endregion


    }


    public class BSEngine
    {
        public BSTypeDatabase TypeDatabase { get; }
        private static readonly List < BSExpressionValidator > s_Validators =
            new List < BSExpressionValidator > { new BSFunctionReturnExpressionValidator() };

        private readonly Dictionary < string, ABSObject > m_Preprocessors;
        private readonly ABSTable m_StaticData;
        private readonly ABSTable m_GlobalTable;
        private readonly List < ABSScriptInterface > m_Interfaces;

        public BSParserSettings ParserSettings { get; }

        public string[] InterfaceNames => m_Interfaces.Select( x => x.Name ).ToArray();

        #region Public

        public BSEngine(
            BSParserSettings parserSettings,
            Dictionary < string, ABSObject > startObjects,
            List < ABSScriptInterface > interfaces,
            Dictionary < string, ABSObject > gTable = null )
        {
            TypeDatabase = new BSTypeDatabase();
            ParserSettings = parserSettings;
            m_Interfaces = interfaces;
            m_Preprocessors = new Dictionary < string, ABSObject >();

            Dictionary < ABSObject, ABSObject > staticData =
                new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, ABSObject > buildScriptEngineRuntimeObject in startObjects )
            {
                staticData[new BSObject( SourcePosition.Unknown, buildScriptEngineRuntimeObject.Key )] =
                    buildScriptEngineRuntimeObject.Value;
            }

            BSTable sd = new BSTable( SourcePosition.Unknown, staticData );

            sd.Lock();
            m_StaticData = sd;

            if ( gTable == null )
            {
                m_GlobalTable = new BSTable( SourcePosition.Unknown );
            }
            else
            {
                Dictionary < ABSObject, ABSObject > globalTable =
                    new Dictionary < ABSObject, ABSObject >();

                foreach ( KeyValuePair < string, ABSObject > buildScriptEngineRuntimeObject in gTable )
                {
                    globalTable[new BSObject( SourcePosition.Unknown, buildScriptEngineRuntimeObject.Key )] =
                        buildScriptEngineRuntimeObject.Value;
                }

                m_GlobalTable = new BSTable( SourcePosition.Unknown, globalTable );
            }

            m_GlobalTable.InsertElement( new BSObject( "__G" ), m_GlobalTable );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static BSExpression[] ParseBinary( byte[] bin )
        {
            using ( Stream ms = new MemoryStream( bin ) )
            {
                return BSSerializer.Deserialize( ms );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static BSExpression[] ParseBinary( string file )
        {
            return ParseBinary( File.ReadAllBytes( file ) );
        }

        public void AddInterface( ABSScriptInterface i )
        {
            m_Interfaces.Add( i );
        }

        public ABSTable GetGlobalTable()
        {
            return m_GlobalTable;
        }

        public bool HasInterface( string key )
        {
            return m_Interfaces.Any( x => x.Name == key );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), scope, args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, BSScope scope, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), scope, args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), scope, args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), scope, args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, ABSObject[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, BSScope scope, string[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), scope, args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), scope, args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, string[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSTable LoadInterface( string key, ABSTable t = null )
        {
            if ( !HasInterface( key ) )
            {
                throw new BSRuntimeException( $"Can not find interface: '{key}'" );
            }

            ABSTable MakeInterfaceTable()
            {
                ABSTable it = new BSTable( SourcePosition.Unknown );

                m_GlobalTable.InsertElement( new BSObject( key ), it );

                return it;
            }

            ABSTable table = t ?? MakeInterfaceTable();

            List < ABSScriptInterface > i = m_Interfaces.Where( x => x.Name == key ).ToList();
            i.ForEach( x => x.AddApi( table ) );

            return table;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, bool isBenchmark = false )
        {
            return LoadScript( exprs, new ABSObject[0], isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, string[] args, bool isBenchmark = false )
        {
            return LoadScript(
                              exprs,
                              args?.Select( x => ( ABSObject )new BSObject( x ) ).ToArray() ?? new ABSObject[0],
                              isBenchmark
                             );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( exprs, new BSScope( this ), args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, BSScope scope, string[] args, bool isBenchmark = false )
        {
            return LoadScript(
                              exprs,
                              scope,
                              args?.Select( x => ( ABSObject )new BSObject( x ) ).ToArray() ?? new ABSObject[0],
                              isBenchmark
                             );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            if ( ParserSettings.AllowOptimization )
            {
                BSExpressionOptimizer.Optimize( exprs );
            }

            scope.AddLocalVar(
                              "args",
                              args == null
                                  ? ( ABSObject )BSObject.Null
                                  : new BSArray( args )
                             );

            Stopwatch sw = null;

            if ( isBenchmark )
            {
                sw = Stopwatch.StartNew();
            }

            foreach ( BSExpression buildScriptExpression in exprs )
            {
                buildScriptExpression.Execute( scope );

                if ( scope.BreakExecution )
                {
                    break;
                }
            }

            if ( isBenchmark )
            {
                sw.Stop();
                Console.WriteLine( $"[BS Benchmark] Execution took: {sw.ElapsedMilliseconds}ms ({sw.Elapsed})" );
            }

            return scope.Return ?? scope.GetLocals();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), scope, args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, BSScope scope, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), scope, args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), args, isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), isBenchmark );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public BSExpression[] ParseFile( string path )
        {
            return ParseString( File.ReadAllText( path ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public BSExpression[] ParseString( string script )
        {
            BSParser parser = new BSParser( Preprocess( script ) );
            BSExpression[] exprs = parser.ParseToEnd();

            foreach ( BSExpressionValidator bsExpressionValidator in s_Validators )
            {
                bsExpressionValidator.Validate( exprs );
            }

            return exprs;
        }

        internal ABSObject AddPreprocessorApi( ABSObject[] arg )
        {
            string name = arg[0].ConvertString();
            ABSObject preprocessor = arg[1];

            if ( preprocessor.HasProperty( "preprocess" ) )
            {
                m_Preprocessors[name] = preprocessor.GetProperty( "preprocess" ).ResolveReference();
            }
            else
            {
                Console.WriteLine(
                                  $"PreProcessor '{name}' does not define function 'preprocess(src)'\nPassed Value: {preprocessor.SafeToString()}"
                                 );
            }

            return BSObject.Null;
        }

        internal ABSObject CreateScope( ABSObject[] args )
        {
            BSScope scope;

            if ( args.Length == 1 )
            {
                scope = new BSScope(
                                    BSScopeFlags.None,
                                    ( BSScope )( args[0].ResolveReference() as BSObject ).GetInternalObject()
                                   );
            }
            else
            {
                scope = new BSScope( this );
            }

            return new BSObject( scope );
        }

        internal ABSObject GetElement( ABSObject name )
        {
            if ( m_GlobalTable.HasElement( name ) )
            {
                return m_GlobalTable.GetElement( name );
            }

            if ( m_StaticData.HasElement( name ) )
            {
                return m_StaticData.GetElement( name );
            }

            throw new BSRuntimeException( name.Position, $"Can not Resolve name: '{name.SafeToString()}'" );
        }

        internal ABSObject GetInterfaceNamesApi( ABSObject[] arg )
        {
            return new BSArray( InterfaceNames.Select( x => new BSObject( x ) ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        internal bool HasElement( ABSObject name )
        {
            return m_GlobalTable.HasElement( name ) || m_StaticData.HasElement( name );
        }

        internal ABSObject HasInterfaceName( ABSObject[] arg )
        {
            return InterfaceNames.Contains( arg[0].ConvertString() ) ? BSObject.True : BSObject.False;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        internal void InsertElement( ABSObject k, ABSObject v )
        {
            m_GlobalTable.InsertElement( k, v );
        }

        internal ABSObject LoadInterfaceApi( ABSObject[] arg )
        {
            string key = arg[0].ConvertString();

            if ( arg.Length == 2 )
            {
                return LoadInterface( key, ( ABSTable )arg[1] );
            }

            return LoadInterface( key );
        }

        internal ABSObject LoadStringApi( ABSObject[] arg )
        {
            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string src ) )
            {
                ABSObject ret = LoadSource( src, arg.Skip( 1 ).ToArray() );

                return ret;
            }

            throw new BSInvalidTypeException(
                                             o.Position,
                                             "Expected String",
                                             o,
                                             "string"
                                            );
        }

        internal ABSObject LoadStringScopedApi( ABSObject[] arg )
        {
            ABSObject scope = arg[0].ResolveReference();
            ABSObject o = arg[1].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                if ( scope is BSObject obj )
                {
                    BSScope sc = ( BSScope )obj.GetInternalObject();

                    ABSObject ret = LoadScript(
                                               ParseString( path ),
                                               sc,
                                               arg.Skip( 2 ).ToArray()
                                              );

                    return ret;
                }

                throw new BSRuntimeException(
                                             o.Position,
                                             "'function loadScopedString(scope, script, args..)' requires a valid scope to be passed as first argument"
                                            );
            }

            throw new BSInvalidTypeException(
                                             o.Position,
                                             "Expected String",
                                             o,
                                             "string"
                                            );
        }

        internal ABSObject LoadStringScopedBenchmarkApi( ABSObject[] arg )
        {
            ABSObject scope = arg[0].ResolveReference();
            ABSObject o = arg[1].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                if ( scope is BSObject obj )
                {
                    BSScope sc = ( BSScope )obj.GetInternalObject();

                    ABSObject ret = LoadScript(
                                               ParseString( path ),
                                               sc,
                                               arg.Skip( 2 ).ToArray(),
                                               true
                                              );

                    return ret;
                }

                throw new BSRuntimeException(
                                             o.Position,
                                             "'function loadScopedString(scope, script, args..)' requires a valid scope to be passed as first argument"
                                            );
            }

            throw new BSInvalidTypeException(
                                             o.Position,
                                             "Expected String",
                                             o,
                                             "string"
                                            );
        }

        #endregion

        #region Private

        private bool IsBinary( string p )
        {
            return Path.GetExtension( p ) == ".cbs";
        }

        private ABSObject LoadStringBenchmarkApi( ABSObject[] arg )
        {
            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                ABSObject ret = LoadSource( path, arg.Skip( 1 ).ToArray(), true );

                return ret;
            }

            throw new BSInvalidTypeException(
                                             o.Position,
                                             "Expected String",
                                             o,
                                             "string"
                                            );
        }

        private string Preprocess( string script )
        {
            ABSObject current = new BSObject( script.Replace( "\r", "" ) );

            foreach ( KeyValuePair < string, ABSObject > preprocessor in m_Preprocessors )
            {
                current = preprocessor.Value.Invoke( new[] { current } );
            }

            return current.ConvertString();
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using BadScript.Exceptions;
using BadScript.Interfaces;
using BadScript.Namespaces;
using BadScript.Optimization;
using BadScript.Parser;
using BadScript.Parser.Expressions;
using BadScript.Scopes;
using BadScript.Serialization;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;
using BadScript.Validators;

namespace BadScript
{

    /// <summary>
    ///     BSEngine is the root object of every BS Runtime Instance.
    ///     BSEngine ties together the Parsing and the Execution of Scripts into a single class.
    /// </summary>
    public class BSEngine
    {

        private static readonly List < BSExpressionValidator > s_Validators =
            new List < BSExpressionValidator > { new BSFunctionReturnExpressionValidator() };

        private readonly Dictionary < string, ABSObject > m_Preprocessors;
        //private readonly ABSTable m_StaticData;
        private readonly ABSTable m_GlobalTable;
        private readonly List < ABSScriptInterface > m_Interfaces;

        /// <summary>
        ///     The namespace Root for all namespaces defined in scripts.
        ///     When a type gets defined outside a namespace, they are saved in this namespace.
        /// </summary>
        public BSNamespaceRoot NamespaceRoot { get; }


        /// <summary>
        ///     Contains the names of all available ABSScriptInterface instances
        /// </summary>
        public string[] InterfaceNames => m_Interfaces.Select( x => x.Name ).ToArray();

        #region Public

        /// <summary>
        /// </summary>
        /// <param name="parserSettings">Settings for the BSParser instance</param>
        /// <param name="startObjects">Static data that is readonly and is available at all Scope Levels</param>
        /// <param name="interfaces">Available Interfaces</param>
        /// <param name="gTable">
        ///     Optional Global Table Definition. Can be used to add default elements before the BSEngine gets
        ///     created.
        /// </param>
        public BSEngine(
            Dictionary < string, ABSObject > startObjects,
            List < ABSScriptInterface > interfaces)
        {
            NamespaceRoot = new BSNamespaceRoot();
            m_Interfaces = interfaces;
            m_Preprocessors = new Dictionary < string, ABSObject >();

            Dictionary < ABSObject, ABSObject > staticData =
                new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, ABSObject > buildScriptEngineRuntimeObject in startObjects )
            {
                staticData[new BSObject( SourcePosition.Unknown, buildScriptEngineRuntimeObject.Key )] =
                    buildScriptEngineRuntimeObject.Value;
            }

            m_GlobalTable = new BSTable(SourcePosition.Unknown, staticData);

            m_GlobalTable.InsertElement( new BSObject( "__G" ), m_GlobalTable );
        }

        /// <summary>
        ///     Creates an Executable BSExpression Tree from the BSBinary Format
        /// </summary>
        /// <param name="bin">Binary Data</param>
        /// <returns>Expression Tree</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static BSExpression[] ParseBinary( byte[] bin )
        {
            using ( Stream ms = new MemoryStream( bin ) )
            {
                return BSSerializer.Deserialize( ms );
            }
        }

        /// <summary>
        ///     Creates an Executable BSExpression Tree from a serialized file
        /// </summary>
        /// <param name="file">Path to File</param>
        /// <returns>Expression Tree</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static BSExpression[] ParseBinary( string file )
        {
            return ParseBinary( File.ReadAllBytes( file ) );
        }

        /// <summary>
        ///     Adds an Interface to the list of available interfaces
        /// </summary>
        /// <param name="i"></param>
        public void AddInterface( ABSScriptInterface i )
        {
            m_Interfaces.Add( i );
        }

        /// <summary>
        ///     Returns the Global Table for this engine instance
        /// </summary>
        /// <returns></returns>
        public ABSTable GetGlobalTable()
        {
            return m_GlobalTable;
        }

        public bool HasInterface( string key )
        {
            return m_Interfaces.Any( x => x.Name == key );
        }

        /// <summary>
        ///     Loads and Executes a Binary
        /// </summary>
        /// <param name="bin">Binary Data</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), scope, args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a Binary
        /// </summary>
        /// <param name="bin">Binary Data</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a Binary
        /// </summary>
        /// <param name="bin">Binary Data</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, BSScope scope, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), scope, args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a Binary
        /// </summary>
        /// <param name="bin">Binary Data</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a Binary
        /// </summary>
        /// <param name="bin">Binary Data</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadBinary( byte[] bin, bool isBenchmark = false )
        {
            return LoadScript( ParseBinary( bin ), isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a File
        /// </summary>
        /// <param name="path">File Path</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), scope, args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), scope, args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a File
        /// </summary>
        /// <param name="path">File Path</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, ABSObject[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a File
        /// </summary>
        /// <param name="path">File Path</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, BSScope scope, string[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), scope, args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), scope, args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a File
        /// </summary>
        /// <param name="path">File Path</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, string[] args, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), args, isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a File
        /// </summary>
        /// <param name="path">File Path</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadFile( string path, bool isBenchmark = false )
        {
            if ( IsBinary( path ) )
            {
                return LoadScript( ParseBinary( path ), isBenchmark );
            }

            return LoadSource( File.ReadAllText( path ), isBenchmark );
        }

        /// <summary>
        ///     Loads an interface and returns the loaded table.
        /// </summary>
        /// <param name="key">Interface Key</param>
        /// <param name="t">Optional Table Instance that the interface is loaded in</param>
        /// <returns>The Loaded Interface</returns>
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

        /// <summary>
        ///     Loads and Executes an BSExpression Tree
        /// </summary>
        /// <param name="exprs">The Expressions to Execute</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, bool isBenchmark = false )
        {
            return LoadScript( exprs, Array.Empty < ABSObject >(), isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes an BSExpression Tree
        /// </summary>
        /// <param name="exprs">The Expressions to Execute</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, string[] args, bool isBenchmark = false )
        {
            return LoadScript(
                              exprs,
                              args?.Select( x => ( ABSObject )new BSObject( x ) ).ToArray() ?? Array.Empty < ABSObject >(),
                              isBenchmark
                             );
        }

        /// <summary>
        ///     Loads and Executes an BSExpression Tree
        /// </summary>
        /// <param name="exprs">The Expressions to Execute</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( exprs, new BSScope( this ), args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes an BSExpression Tree
        /// </summary>
        /// <param name="exprs">The Expressions to Execute</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, BSScope scope, string[] args, bool isBenchmark = false )
        {
            return LoadScript(
                              exprs,
                              scope,
                              args?.Select( x => ( ABSObject )new BSObject( x ) ).ToArray() ?? Array.Empty < ABSObject >(),
                              isBenchmark
                             );
        }

        /// <summary>
        ///     Loads and Executes an BSExpression Tree
        /// </summary>
        /// <param name="exprs">The Expressions to Execute</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadScript( BSExpression[] exprs, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            if ( BSEngineSettings.ENABLE_OPTIMIZE_CONST_EXPRESSIONS)
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

        /// <summary>
        ///     Loads and Executes a script from source
        /// </summary>
        /// <param name="src">The Source to Execute</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, BSScope scope, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), scope, args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a script from source
        /// </summary>
        /// <param name="src">The Source to Execute</param>
        /// <param name="scope">The Scope that the Execution will take place in</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, BSScope scope, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), scope, args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a script from source
        /// </summary>
        /// <param name="src">The Source to Execute</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, string[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a script from source
        /// </summary>
        /// <param name="src">The Source to Execute</param>
        /// <param name="args">Startup Arguments(available as 'args' inside the script)</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, ABSObject[] args, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), args, isBenchmark );
        }

        /// <summary>
        ///     Loads and Executes a script from source
        /// </summary>
        /// <param name="src">The Source to Execute</param>
        /// <param name="isBenchmark">Display Execution Time</param>
        /// <returns>Return of the Execution. The ABSTable instance of the specified scope if no explicit return inside the script</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ABSObject LoadSource( string src, bool isBenchmark = false )
        {
            return LoadScript( ParseString( src ), isBenchmark );
        }

        /// <summary>
        ///     Parses a BS Expression Tree from file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public BSExpression[] ParseFile( string path )
        {
            return ParseString( File.ReadAllText( path ) );
        }

        /// <summary>
        ///     Parses a BS Expression Tree from source
        /// </summary>
        /// <param name="script">The source to parse</param>
        /// <returns></returns>
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
        internal ABSObject ResetScope(ABSObject[] args)
        {
            BSScope scope = (BSScope)((BSObject)args[0].ResolveReference()).GetInternalObject();

            scope.ResetFlag();

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
            

            throw new BSRuntimeException( name.Position, $"Can not Resolve name: '{name.SafeToString()}'" );
        }

        internal ABSObject GetInterfaceNamesApi( ABSObject[] arg )
        {
            return new BSArray( InterfaceNames.Select( x => new BSObject( x ) ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        internal bool HasElement( ABSObject name )
        {
            return m_GlobalTable.HasElement( name );
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

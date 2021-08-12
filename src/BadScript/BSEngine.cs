﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BadScript.Common;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;
using BadScript.Interfaces;
using BadScript.Settings;
using BadScript.Utils.Optimization;

namespace BadScript
{

    public class BSEngine
    {
        public readonly BSParserSettings ParserSettings;
        private readonly Dictionary < string, ABSObject > m_Preprocessors;
        private readonly ABSTable m_StaticData;
        private readonly ABSTable m_GlobalTable;
        private readonly List < ABSScriptInterface > m_Interfaces;

        public string[] InterfaceNames => m_Interfaces.Select( x => x.Name ).ToArray();

        #region Public

        public BSEngine(
            BSParserSettings parserSettings,
            Dictionary < string, ABSObject > startObjects,
            List < ABSScriptInterface > interfaces,
            Dictionary < string, ABSObject > gTable = null )
        {
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

        public ABSObject LoadFile(bool isBenchmark, string file, string[] args)
        {
            return LoadString(isBenchmark, File.ReadAllText(file), args);
        }
        public ABSObject LoadFile(bool isBenchmark, string file, ABSObject[] args)
        {
            return LoadString(isBenchmark, File.ReadAllText(file), args);
        }
        public ABSObject LoadFile(bool isBenchmark, string file)
        {
            return LoadFile(isBenchmark, file, new string[0]);
        }


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

        public ABSObject LoadString(bool isBenchmark, string script, string[] args)
        {
            return LoadString(
                isBenchmark,
                script,
                args?.Select(x => (ABSObject)new BSObject(x)).ToArray()
            );
        }
        public ABSObject LoadString(bool isBenchmark, string script)
        {
            return LoadString(
                isBenchmark,
                script,
                new ABSObject[0]
            );
        }

        public ABSObject LoadString(bool isBenchmark, BSScope scope, string script, string[] args)
        {
            return LoadString(
                isBenchmark,
                scope,
                script,
                args?.Select(x => (ABSObject)new BSObject(x)).ToArray()
            );
        }
        public ABSObject LoadString(bool isBenchmark, BSScope scope, string script)
        {
            return LoadString(
                isBenchmark,
                scope,
                script,
                new ABSObject[0]
            );
        }

        public ABSObject LoadString( bool isBenchmark, BSScope scope, string script, ABSObject[] args )
        {
            BSParser parser = new BSParser( Preprocess( script ) );
            BSExpression[] exprs = parser.ParseToEnd();

            if ( ParserSettings.AllowOptimization )
            {
                BSExpressionOptimizer.Optimize( exprs );
            }

            scope.AddLocalVar(
                "args",
                args == null
                    ? ( ABSObject ) BSObject.Null
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

        public ABSObject LoadString(bool isBenchmark, string script, ABSObject[] args)
        {
            return LoadString(isBenchmark, new BSScope(this), script, args);
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
                    $"PreProcessor '{name}' does not define function 'preprocess(src)'\nPassed Value: {preprocessor.SafeToString()}" );
            }

            return BSObject.Null;
        }

        internal ABSObject CreateScope( ABSObject[] args )
        {
            BSScope scope;

            if ( args.Length == 1 )
            {
                scope = new BSScope( BSScopeFlags.None, ( BSScope ) ( args[0] as BSObject ).GetInternalObject() );
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

        internal bool HasElement( ABSObject name )
        {
            return m_GlobalTable.HasElement( name ) || m_StaticData.HasElement( name );
        }

        internal ABSObject HasInterfaceName( ABSObject[] arg )
        {
            return InterfaceNames.Contains( arg[0].ConvertString() ) ? BSObject.One : BSObject.Zero;
        }

        internal void InsertElement( ABSObject k, ABSObject v )
        {
            m_GlobalTable.InsertElement( k, v );
        }

        internal ABSObject LoadInterfaceApi( ABSObject[] arg )
        {
            string key = arg[0].ConvertString();

            if ( arg.Length == 2 )
            {
                return LoadInterface( key, ( ABSTable ) arg[1] );
            }

            return LoadInterface( key );
        }

        internal ABSObject LoadStringApi( ABSObject[] arg )
        {
            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                ABSObject ret = LoadString( false, path, arg.Skip( 1 ).ToArray() );

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
                    BSScope sc = ( BSScope ) obj.GetInternalObject();
                    ABSObject ret = LoadString( false, sc, path, arg.Skip( 2 ).ToArray() );

                    return ret;
                }

                throw new BSRuntimeException(
                    o.Position,
                    "'function loadScopedString(scope, script, args..)' requires a valid scope to be passed as first argument" );

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
                    BSScope sc = ( BSScope ) obj.GetInternalObject();
                    ABSObject ret = LoadString( true, sc, path, arg.Skip( 2 ).ToArray() );

                    return ret;
                }

                throw new BSRuntimeException(
                    o.Position,
                    "'function loadScopedString(scope, script, args..)' requires a valid scope to be passed as first argument" );

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

        private ABSObject LoadStringBenchmarkApi( ABSObject[] arg )
        {
            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                ABSObject ret = LoadString( true, path, arg.Skip( 1 ).ToArray() );

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

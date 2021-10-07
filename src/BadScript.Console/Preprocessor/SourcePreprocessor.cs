using System;
using System.Collections.Generic;
using System.Linq;

using BadScript.Console.Preprocessor.Directives;
using BadScript.Console.Subsystems.Project;
using BadScript.ConsoleUtils;
using BadScript.Http;
using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Parser;
using BadScript.Process;
using BadScript.StringUtils;
using BadScript.Types;
using BadScript.Xml;
using BadScript.Zip;

namespace BadScript.Console.Preprocessor
{

    public static class SourcePreprocessor
    {

        private static readonly List < SourcePreprocessorDirective > s_Directives =
            new List < SourcePreprocessorDirective >
            {
                new DefinePreprocessorDirective(),
                new IfPreprocessorDirective(),
                new IfDefinedPreprocessorDirective(),
                new IfNotDefinedPreprocessorDirective(),
                new CustomFunctionPreprocessorDirective(),
                new ForPreprocessorDirective(),
                new CustomMacroPreprocessorDirective()
            };

        #region Public

        public static bool IsName( string source, int idx, string name )
        {
            return idx + name.Length <= source.Length &&
                   source.Substring( idx, name.Length ) == name &&
                   IsNonWordChar( source, idx + name.Length );
        }

        public static bool IsNonWordChar( string source, int idx )
        {
            return idx >= source.Length || !char.IsLetterOrDigit( source[idx] ) && source[idx] != '_';
        }

        public static string Preprocess( string source, string directives )
        {
            SourcePreprocessorContext ctx =
                new SourcePreprocessorContext( CreateEngine(), source, directives, s_Directives );

            return Preprocess( ctx );
        }

        public static string Preprocess( SourcePreprocessorContext ctx )
        {
            ctx.ScriptEngine.LoadSource( ctx.DirectivesNames, ctx.RuntimeScope, Array.Empty < ABSObject >() );

            int current = 0;

            while ( current < ctx.OriginalSource.Length )
            {
                ( SourcePreprocessorDirective dir, int next ) = FindNext( ctx, current );

                if ( dir == null )
                {
                    break;
                }

                BSParser p = new BSParser( ctx.OriginalSource );

                p.SetPosition( next );
                string output = dir.Process( p, ctx );

                ctx.RuntimeScope.ResetFlag(); //Reset Return Flags

                int inLength = p.GetPosition() - next;

                ctx.OriginalSource = ctx.OriginalSource.Remove( next, inLength ).Insert( next, output );

                current = next;
            }

            return ctx.OriginalSource;
        }

        #endregion

        #region Private

        private static BSEngine CreateEngine()
        {
            BSEngineSettings settings = BSEngineSettings.MakeDefault();
            settings.IncludeDirectories.Clear();
            settings.IncludeDirectories.Add( ProjectSystemDirectories.Instance.PreprocessorIncludeDirectory );

            settings.Interfaces.Add( new BSSystemConsoleInterface() );
            settings.Interfaces.Add( new BSCollectionInterface() );
            settings.Interfaces.Add( new BSConvertInterface() );
            settings.Interfaces.Add( new BSHttpInterface() );
            settings.Interfaces.Add( new Json2BSInterface() );
            settings.Interfaces.Add( new BS2JsonInterface() );
            settings.Interfaces.Add( new BSXmlInterface() );
            settings.Interfaces.Add( new BSZipInterface() );
            settings.Interfaces.Add( new BSMathInterface() );
            settings.Interfaces.Add( new BSProcessInterface() );
            settings.Interfaces.Add( new BSFileSystemInterface() );
            settings.Interfaces.Add( new BSFileSystemPathInterface( AppDomain.CurrentDomain.BaseDirectory ) );

            settings.Interfaces.Add( new BSStringInterface() );
            settings.ActiveInterfaces.Add( "string" );

            return settings.Build();
        }

        private static (SourcePreprocessorDirective, int) FindNext( SourcePreprocessorContext ctx, int current )
        {
            for ( int i = current; i < ctx.OriginalSource.Length; i++ )
            {
                SourcePreprocessorDirective dir =
                    ctx.Directives.FirstOrDefault( x => IsName( ctx.OriginalSource, i, x.Name ) );

                if ( dir != null )
                {
                    return ( dir, i );
                }
            }

            return ( null, -1 );
        }

        #endregion

    }

}

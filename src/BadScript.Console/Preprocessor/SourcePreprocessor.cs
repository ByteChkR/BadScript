using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Common;
using BadScript.Common.Types;
using BadScript.Console.Preprocessor.Directives;
using BadScript.Console.Subsystems.Project;
using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Settings;
using BadScript.StringUtils;


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
            };

        

        private static BSEngine CreateEngine()
        {
            BSEngineSettings settings = BSEngineSettings.MakeDefault();
            settings.IncludeDirectories.Clear();
            settings.IncludeDirectories.Add(ProjectSystemDirectories.Instance.PreprocessorIncludeDirectory);

            settings.Interfaces.Add(new ConsoleApi());
            settings.Interfaces.Add(new BadScriptCoreApi());

            settings.Interfaces.Add( new StringUtilsApi() );
            settings.ActiveInterfaces.Add( "string" );
            

            return settings.Build();
        }

        private static bool IsName( string source, int idx, string name )
        {
            return idx + name.Length <= source.Length &&
                   source.Substring( idx, name.Length ) == name &&
                   IsNonWordChar( source, idx + name.Length );
        }

        private static bool IsNonWordChar( string source, int idx ) =>
            idx >= source.Length || !char.IsLetterOrDigit( source[idx] ) && source[idx] != '_';
        private static (SourcePreprocessorDirective, int) FindNext( SourcePreprocessorContext ctx,  int current )
        {
            for ( int i = current; i < ctx.OriginalSource.Length; i++ )
            {
                SourcePreprocessorDirective dir = ctx.Directives.FirstOrDefault( x => IsName(ctx.OriginalSource, i, x.Name ) );

                if ( dir != null )
                    return ( dir, i );
            }

            return ( null, -1 );
        }

        public static string Preprocess( string source, string directives )
        {
            SourcePreprocessorContext ctx = new SourcePreprocessorContext(CreateEngine(), source, s_Directives);

            ctx.ScriptEngine.LoadSource( directives, ctx.RuntimeScope, Array.Empty < ABSObject >() );


            int current = 0;

            while ( current < source.Length )
            {
                ( SourcePreprocessorDirective dir, int next ) = FindNext(ctx, current );

                if ( dir == null )
                    break;

                BSParser p = new BSParser( ctx.OriginalSource );

                p.SetPosition( next );
                string output = dir.Process( p , ctx);

                ctx.RuntimeScope.ResetFlag(); //Reset Return Flags

                int inLength = p.GetPosition()-next;

                ctx.OriginalSource = ctx.OriginalSource.Remove( next, inLength ).Insert( next, output );

                current = next;
            }

            return ctx.OriginalSource;
        }

    }

}

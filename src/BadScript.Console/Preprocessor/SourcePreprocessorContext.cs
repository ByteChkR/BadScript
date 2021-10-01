using System.Collections.Generic;

using BadScript.Common.Runtime;

namespace BadScript.Console.Preprocessor
{

    public class SourcePreprocessorContext
    {

        public BSEngine ScriptEngine { get; }
        public BSScope RuntimeScope { get; }
        public string OriginalSource { get; set; }
        public string DirectivesNames { get; }
        public List<SourcePreprocessorDirective> Directives { get; }

        public SourcePreprocessorContext(
            BSEngine engine,
            string src,
            string directiveNames,
            IEnumerable < SourcePreprocessorDirective > directives ) : this(
                                                                            engine,
                                                                            new BSScope( engine ),
                                                                            src,
                                                                            directiveNames,
                                                                            directives
                                                                           ) { }

        private SourcePreprocessorContext(
            BSEngine engine,
            BSScope scope,
            string src,
            string directiveNames,
            IEnumerable < SourcePreprocessorDirective > directives )
        {
            ScriptEngine = engine;
            OriginalSource = src;
            DirectivesNames = directiveNames;
            RuntimeScope = scope;
            Directives = new List<SourcePreprocessorDirective>(directives);
        }

        public SourcePreprocessorContext CreateSubContext( string src ) =>
            new SourcePreprocessorContext( ScriptEngine, RuntimeScope, src, DirectivesNames, Directives );

    }

}
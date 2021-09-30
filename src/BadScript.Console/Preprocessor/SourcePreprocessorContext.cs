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

        public SourcePreprocessorContext( BSEngine engine, string src, string directiveNames, IEnumerable<SourcePreprocessorDirective> directives )
        {
            ScriptEngine = engine;
            OriginalSource = src;
            DirectivesNames = directiveNames;
            RuntimeScope = new BSScope(ScriptEngine);
            Directives = new List < SourcePreprocessorDirective >( directives );
        }
    }

}
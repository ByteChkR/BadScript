using System.Collections.Generic;

using BadScript.Common.Runtime;

namespace BadScript.Console.Preprocessor
{

    public class SourcePreprocessorContext
    {

        public BSEngine ScriptEngine { get; }
        public BSScope RuntimeScope { get; }
        public string OriginalSource { get; set; }
        public List<SourcePreprocessorDirective> Directives { get; }

        public SourcePreprocessorContext( BSEngine engine, string src, IEnumerable<SourcePreprocessorDirective> directives )
        {
            ScriptEngine = engine;
            OriginalSource = src;
            RuntimeScope = new BSScope(ScriptEngine);
            Directives = new List < SourcePreprocessorDirective >( directives );
        }
    }

}
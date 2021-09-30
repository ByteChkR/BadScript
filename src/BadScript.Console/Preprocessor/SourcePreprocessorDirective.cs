using BadScript.Common;

namespace BadScript.Console.Preprocessor
{

    public abstract class SourcePreprocessorDirective
    {

        public virtual string Name { get; }

        protected SourcePreprocessorDirective(){}

        protected SourcePreprocessorDirective( string name )
        {
            Name = name;
        }

        public abstract string Process(BSParser p, SourcePreprocessorContext ctx);

    }

}
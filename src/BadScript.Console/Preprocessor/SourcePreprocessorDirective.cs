using BadScript.Parser;

namespace BadScript.Console.Preprocessor
{

    public abstract class SourcePreprocessorDirective
    {

        public virtual string Name { get; }

        #region Public

        public abstract string Process( BSParser p, SourcePreprocessorContext ctx );

        #endregion

        #region Protected

        protected SourcePreprocessorDirective()
        {
        }

        protected SourcePreprocessorDirective( string name )
        {
            Name = name;
        }

        #endregion

    }

}

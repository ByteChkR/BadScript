using BadScript.Parser;
using BadScript.Parser.Expressions;

namespace BadScript.Console.Preprocessor.Directives
{

    public class DefinePreprocessorDirective : SourcePreprocessorDirective
    {

        #region Public

        public DefinePreprocessorDirective() : base( "#define" )
        {
        }

        public override string Process( BSParser p, SourcePreprocessorContext ctx )
        {
            p.SetPosition( p.GetPosition() + Name.Length ); //Skip #define

            BSExpression expr = p.Parse( int.MaxValue );

            expr.Execute( ctx.RuntimeScope );

            return string.Empty;
        }

        #endregion

    }

}

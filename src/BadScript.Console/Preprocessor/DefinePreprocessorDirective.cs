using BadScript.Common;
using BadScript.Common.Expressions;

namespace BadScript.Console.Preprocessor
{

    public class DefinePreprocessorDirective : SourcePreprocessorDirective
    {

        public DefinePreprocessorDirective() : base("#define") { }

        public override string Process(BSParser p, SourcePreprocessorContext ctx)
        {
            p.SetPosition(p.GetPosition() + Name.Length); //Skip #define

            BSExpression expr = p.Parse(int.MaxValue);

            expr.Execute(ctx.RuntimeScope);

            return string.Empty;
        }

    }

}
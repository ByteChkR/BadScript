using BadScript.Common;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Types;

namespace BadScript.Console.Preprocessor.Directives
{

    public class IfPreprocessorDirective : SourcePreprocessorDirective
    {

        public IfPreprocessorDirective() : base("#if") { }

        public override string Process(BSParser p, SourcePreprocessorContext ctx)
        {
            int pos = p.GetPosition() + Name.Length;
            p.SetPosition(pos); //Skip #define

            BSIfExpression expr = p.ParseIfExpression(pos, s=> SourcePreprocessor.Preprocess(s, ctx.DirectivesNames));

            expr.Execute(ctx.RuntimeScope);

            ABSObject o = ctx.RuntimeScope.Return;

            return o == null || o.IsNull ? "" : o.ConvertString()+"\n";
        }

    }

}
using BadScript.Common;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Types;

namespace BadScript.Console.Preprocessor.Directives
{

    public class IfDefinedPreprocessorDirective : SourcePreprocessorDirective
    {

        public IfDefinedPreprocessorDirective() : base("#ifdef") { }

        public override string Process(BSParser p, SourcePreprocessorContext ctx)
        {
            int pos = p.GetPosition() + Name.Length;
            p.SetPosition(pos); //Skip #define
            p.ReadWhitespaceAndNewLine();
            string def = p.GetNextWord();

            p.ReadWhitespaceAndNewLine();
            string block = SourcePreprocessor.Preprocess(p.ParseBlock(), ctx.DirectivesNames);

            BSParser sp = new BSParser(block, ctx.OriginalSource, p.GetPosition());
            BSExpression blockExpr = new BSBlockExpression(sp.ParseToEnd());

            if (ctx.RuntimeScope.Has(def))
            {
                blockExpr.Execute(ctx.RuntimeScope);

                ABSObject o = ctx.RuntimeScope.Return;

                return o == null || o.IsNull ? "" : o.ConvertString() + "\n";
            }

            return "";
        }



    }

}
using BadScript.Common;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Types;

namespace BadScript.Console.Preprocessor
{

    public class IfNotDefinedPreprocessorDirective : SourcePreprocessorDirective
    {

        public IfNotDefinedPreprocessorDirective() : base("#ndef") { }

        public override string Process(BSParser p, SourcePreprocessorContext ctx)
        {
            int pos = p.GetPosition() + Name.Length;
            p.SetPosition(pos); //Skip #define
            p.ReadWhitespaceAndNewLine();
            string def = p.GetNextWord();

            p.ReadWhitespaceAndNewLine();
            string block = p.ParseBlock();

            BSParser sp = new BSParser( block, ctx.OriginalSource, p.GetPosition() );
            BSExpression blockExpr = new BSBlockExpression( sp.ParseToEnd() );
            
            if ( !ctx.RuntimeScope.Has( def ) )
            {
                blockExpr.Execute( ctx.RuntimeScope );

                ABSObject o = ctx.RuntimeScope.Return;

                return o == null || o.IsNull ? "" : o.ConvertString() + "\n";
            }

            return "";
        }

    }

}
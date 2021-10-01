using BadScript.Common;
using BadScript.Common.Expressions.Implementations.Block.ForEach;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript.Console.Preprocessor.Directives
{

    public class ForPreprocessorDirective : SourcePreprocessorDirective
    {
        public ForPreprocessorDirective() : base("#for") { }

        public override string Process( BSParser p, SourcePreprocessorContext ctx )
        {
            int pos = p.GetPosition() + Name.Length;
            p.SetPosition(pos); //Skip #define

            p.ReadWhitespaceAndNewLine();
            string aggregateVar = p.GetNextWord();
            p.ReadWhitespaceAndNewLine();


            BSScope aggrScope = new BSScope( BSScopeFlags.None, ctx.RuntimeScope );
            aggrScope.AddLocalVar( aggregateVar, new BSObject("") );



            BSForExpression expr = p.ParseForExpression(pos, s=> SourcePreprocessor.Preprocess(s, ctx.CreateSubContext(s)));

            expr.Execute(aggrScope);

            ABSObject o = aggrScope.Get(aggregateVar).ResolveReference();

            return o == null || o.IsNull ? "" : o.ConvertString() + "\n";
        }

    }

}
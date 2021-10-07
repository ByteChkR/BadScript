using BadScript.Parser;
using BadScript.Parser.Expressions.Implementations.Block.ForEach;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;
using BadScript.Types.References;

namespace BadScript.Console.Preprocessor.Directives
{

    public class ForPreprocessorDirective : SourcePreprocessorDirective
    {

        #region Public

        public ForPreprocessorDirective() : base( "#for" )
        {
        }

        public override string Process( BSParser p, SourcePreprocessorContext ctx )
        {
            int pos = p.GetPosition() + Name.Length;
            p.SetPosition( pos ); //Skip #define

            p.ReadWhitespaceAndNewLine();
            string aggregateVar = p.GetNextWord();
            p.ReadWhitespaceAndNewLine();

            BSScope aggrScope = new BSScope( BSScopeFlags.None, ctx.RuntimeScope );
            aggrScope.AddLocalVar( aggregateVar, new BSObject( "" ) );

            BSForExpression expr = p.ParseForExpression(
                                                        pos,
                                                        s => SourcePreprocessor.Preprocess(
                                                             ctx.CreateSubContext(s)
                                                            )
                                                       );

            expr.Execute( aggrScope );

            ABSObject o = aggrScope.Get( aggregateVar ).ResolveReference();

            return o == null || o.IsNull() ? "" : o.ConvertString() + "\n";
        }

        #endregion

    }

}

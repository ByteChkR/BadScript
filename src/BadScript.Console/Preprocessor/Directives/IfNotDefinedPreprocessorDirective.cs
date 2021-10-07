using BadScript.Parser;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Types;

namespace BadScript.Console.Preprocessor.Directives
{

    public class IfNotDefinedPreprocessorDirective : SourcePreprocessorDirective
    {

        #region Public

        public IfNotDefinedPreprocessorDirective() : base( "#ifndef" )
        {
        }

        public override string Process( BSParser p, SourcePreprocessorContext ctx )
        {
            int pos = p.GetPosition() + Name.Length;
            p.SetPosition( pos ); //Skip #define
            p.ReadWhitespaceAndNewLine();
            string def = p.GetNextWord();

            p.ReadWhitespaceAndNewLine();
            string block = p.ParseBlock();
            block = SourcePreprocessor.Preprocess( ctx.CreateSubContext( block ) );

            BSParser sp = new BSParser( block, ctx.OriginalSource, p.GetPosition() );
            BSExpression blockExpr = new BSBlockExpression( sp.ParseToEnd() );

            if ( !ctx.RuntimeScope.Has( def ) )
            {
                blockExpr.Execute( ctx.RuntimeScope );

                ABSObject o = ctx.RuntimeScope.Return;

                return o == null || o.IsNull() ? "" : o.ConvertString() + "\n";
            }

            return "";
        }

        #endregion

    }

}

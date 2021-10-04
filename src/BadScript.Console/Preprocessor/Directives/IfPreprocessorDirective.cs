using BadScript.Parser;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Types;

namespace BadScript.Console.Preprocessor.Directives
{

    public class IfPreprocessorDirective : SourcePreprocessorDirective
    {

        #region Public

        public IfPreprocessorDirective() : base( "#if" )
        {
        }

        public override string Process( BSParser p, SourcePreprocessorContext ctx )
        {
            int pos = p.GetPosition() + Name.Length;
            p.SetPosition( pos ); //Skip #define

            BSIfExpression expr = p.ParseIfExpression(
                                                      pos,
                                                      s => SourcePreprocessor.Preprocess(ctx.CreateSubContext( s ) )
                                                     );

            expr.Execute( ctx.RuntimeScope );

            ABSObject o = ctx.RuntimeScope.Return;

            return o == null || o.IsNull ? "" : o.ConvertString() + "\n";
        }

        #endregion

    }

}

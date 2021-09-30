﻿using BadScript.Common;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Types;

namespace BadScript.Console.Preprocessor
{

    public class CustomFunctionPreprocessorDirective : SourcePreprocessorDirective
    {

        private class FunctionPreprocessorDirective : SourcePreprocessorDirective
        {
            private readonly BSFunctionDefinitionExpression m_Function;
            public FunctionPreprocessorDirective(BSFunctionDefinitionExpression fnc ):base('#' + fnc.Name)
            {
                m_Function = fnc;
            }

            public override string Process( BSParser p, SourcePreprocessorContext ctx )
            {

                p.SetPosition(p.GetPosition() + Name.Length); //Skip name

                BSExpression expr = p.ParseInvocation(m_Function);
                ABSObject o = expr.Execute(ctx.RuntimeScope);

                return o==null || o.IsNull ? "" : o.ConvertString() + "\n";
            }

        }

        public CustomFunctionPreprocessorDirective() : base("#custom") { }

        public override string Process(BSParser p, SourcePreprocessorContext ctx)
        {
            p.SetPosition(p.GetPosition() + Name.Length); //Skip #define

            BSFunctionDefinitionExpression expr = p.ParseFunction(false);

            ctx.Directives.Add( new FunctionPreprocessorDirective( expr ) );

            return string.Empty;
        }

    }

}
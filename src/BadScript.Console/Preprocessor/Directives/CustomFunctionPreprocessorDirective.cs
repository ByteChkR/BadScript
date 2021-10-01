using System;
using System.Collections.Generic;
using System.Linq;

using BadScript.Common;
using BadScript.Common.Expressions;
using BadScript.Common.Expressions.Implementations.Access;
using BadScript.Common.Expressions.Implementations.Block;
using BadScript.Common.Expressions.Implementations.Value;
using BadScript.Common.Types;

namespace BadScript.Console.Preprocessor.Directives
{

    public class CustomMacroPreprocessorDirective : SourcePreprocessorDirective
    {

        private class MacroPreprocessorDirective : SourcePreprocessorDirective
        {

            private readonly string m_Block;
            private readonly BSFunctionParameter[] m_Parameters;

            public MacroPreprocessorDirective( string name, BSFunctionParameter[] parameters, string block ):base($"#{name}")
            {
                m_Parameters = parameters;
                m_Block = block;
            }
            private string GenerateBlock( string current, BSFunctionParameter parameter, string value )
            {
                bool lastIsWordChar = false;
                for ( int i = 0; i < current.Length; i++ )
                {
                    if ( SourcePreprocessor.IsName( current, i, parameter.Name ) && !lastIsWordChar )
                    {
                        current = current.Remove( i, parameter.Name.Length ).Insert( i, value );
                    }

                    lastIsWordChar = !SourcePreprocessor.IsNonWordChar( current, i );
                }

                return current;
            }

            private string GenerateBlock( string[] values )
            {
                string current = m_Block;

                if (values.Length != m_Parameters.Length )
                {
                    throw new Exception( "Invalid Argument Count for Macro " + Name );
                }
                for ( int i = 0; i < m_Parameters.Length; i++ )
                {
                    current = GenerateBlock( current, m_Parameters[i], values[i] );
                }

                return current;
            }

            private string MakePropertyExpressionString( BSPropertyExpression pexpr )
            {
                string name = pexpr.Right;

                if ( pexpr.Left is BSPropertyExpression prev )
                {
                    return MakePropertyExpressionString( prev ) + "." + name;
                }
                if(pexpr.Left==null)
                {
                    return name;
                } 
                throw new Exception( $"Unsupported Property Expression Left({pexpr.Left} side for Macro: {Name}" );
                
            }

            public override string Process( BSParser p, SourcePreprocessorContext ctx )
            {
                p.SetPosition(p.GetPosition() + Name.Length); //Skip name

                BSInvocationExpression invocation = p.ParseInvocation(null);

                List < string > values = new List < string >();

                foreach ( BSExpression invocationParameter in invocation.Parameters )
                {
                    if ( invocationParameter is BSValueExpression vexpr )
                    {
                        if(vexpr.SourceValue is string str)
                            values.Add( $"\"{str}\"" );
                        else if (vexpr.SourceValue is null)
                        {
                            values.Add("null");
                        }
                        else if (vexpr.SourceValue is bool b)
                        {
                            values.Add(b.ToString().ToLower());
                        }
                        else
                        {
                            values.Add( vexpr.SourceValue.ToString() );
                        }
                    }
                    else if ( invocationParameter is BSPropertyExpression pexpr )
                    {
                        values.Add( MakePropertyExpressionString( pexpr ) );
                    }
                    else
                    {
                        throw new Exception(
                                            "Expression not supported inside a macro parameter: " + invocationParameter
                                           );
                    }
                }


                string block = GenerateBlock( values.ToArray() );
                string ret= SourcePreprocessor.Preprocess(block, ctx.CreateSubContext(block));

                return ret;
            }

        }

        public CustomMacroPreprocessorDirective():base("#macro"){}
        public override string Process( BSParser p, SourcePreprocessorContext ctx )
        {
            p.SetPosition(p.GetPosition() + Name.Length); //Skip name
            p.ReadWhitespaceAndNewLine();
            string name = p.GetNextWord();
            p.ReadWhitespaceAndNewLine();
            BSFunctionParameter[] parameters = p.ParseArgumentList();
            p.ReadWhitespaceAndNewLine();
            string block = p.ParseBlock();

            ctx.Directives.Add( new MacroPreprocessorDirective( name, parameters, block ) );

            return string.Empty;
        }

    }
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

            BSFunctionDefinitionExpression expr = p.ParseFunction(false,s=> SourcePreprocessor.Preprocess(s, ctx.CreateSubContext(s)));

            ctx.Directives.Add( new FunctionPreprocessorDirective( expr ) );

            return string.Empty;
        }

    }

}
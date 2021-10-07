using System.Linq;
using System.Text;

using BadScript.Exceptions;
using BadScript.Parser.Expressions.Implementations.Value;
using BadScript.Scopes;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Parser.Expressions.Implementations.Block
{

    public class BSFunctionDefinitionExpression : BSExpression
    {

        public string Name;
        public bool Global;
        public BSFunctionParameter[] ArgNames;
        public BSExpression[] Block;

        public BSInvocationExpression BaseInvocation;

        public override bool IsConstant => false;

        #region Public

        public BSFunctionDefinitionExpression(
            SourcePosition srcPos,
            string name,
            BSFunctionParameter[] args,
            BSExpression[] block,
            bool addGlobal,
            BSInvocationExpression baseInvocation = null ) : base( srcPos )
        {
            BaseInvocation = baseInvocation;
            Name = name;
            ArgNames = args;
            Block = block;
            Global = addGlobal;

            if ( args.Length > 1 && args[0].IsArgArray )
            {
                throw new BSParserException(
                                            $"Invalid Arguments for function {name}. Can not have a * argument besides other arguments"
                                           );
            }
        }

        public static void ApplyFunctionArguments(
            BSScope scope,
            BSFunctionParameter[] argNames,
            ABSObject[] arg)
        {
            for (int i = 0; i < argNames.Length; i++)
            {
                BSFunctionParameter p = argNames[i];

                if (p.IsArgArray)
                {
                    BSArray a = new BSArray(arg);
                    scope.AddLocalVar(p.Name, a);

                    break;
                }

                if (arg.Length <= i && !p.IsOptional)
                {
                    throw new BSRuntimeException("Missing Argument: " + p.Name);
                }

                if (p.NotNull && (arg.Length <= i || arg[i].IsNull()))
                {
                    throw new BSRuntimeException(
                                                 arg[i].Position,
                                                 $"Parameter '{p.Name}' can not be null or is missing"
                                                );
                }

                scope.AddLocalVar(p.Name, arg.Length <= i ? BSObject.Null : arg[i]);
            }
        }

        public static ABSObject InvokeBlockFunction(
            BSScope scope,
            BSExpression[] block,
            BSFunctionParameter[] argNames,
            ABSObject[] arg )
        {
            ApplyFunctionArguments( scope, argNames, arg );
            foreach ( BSExpression buildScriptExpression in block )
            {
                buildScriptExpression.Execute( scope );

                if ( scope.BreakExecution || scope.Flags == BSScopeFlags.Continue )
                {
                    break;
                }
            }

            return scope.Return;
        }

        public override ABSObject Execute( BSScope scope )
        {
            int min = ArgNames.Count( x => !x.IsOptional );
            int max = ArgNames.Length;

            if ( ArgNames.Length == 1 && ArgNames[0].IsArgArray )
            {
                min = 0;
                max = int.MaxValue;
            }

            BSFunction f =
                new BSFunction(
                               GetHeader(),
                               x => InvokeBlockFunction( scope, x ),
                               min,
                               max
                              );

            if ( string.IsNullOrEmpty( Name ) )
            {
                return f;
            }

            if ( Global )
            {
                scope.AddGlobalVar( Name, f );
            }
            else
            {
                scope.AddLocalVar( Name, f );
            }

            return f;
        }

        public override string ToString()
        {
            return GetHeader();
        }

        #endregion

        #region Private

        private string GetHeader()
        {
            StringBuilder sb = new StringBuilder( $"function {Name}(" );

            for ( int i = 0; i < ArgNames.Length; i++ )
            {
                string argName = ArgNames[i].Name;

                if ( ArgNames[i].IsArgArray )
                {
                    argName = "*" + argName;
                }

                if ( ArgNames[i].NotNull )
                {
                    argName = "!" + argName;
                }

                if ( ArgNames[i].IsOptional )
                {
                    argName = "?" + argName;
                }

                if ( i == 0 )
                {
                    sb.Append( argName );
                }
                else
                {
                    sb.Append( ", " + argName );
                }
            }

            sb.Append( ")" );

            return sb.ToString();
        }

        private ABSObject InvokeBlockFunction( BSScope rootScope, ABSObject[] arg )
        {
            BSScope funcScope = new BSScope( BSScopeFlags.Function, rootScope );

            return InvokeBlockFunction( funcScope, Block, ArgNames, arg ) ?? BSObject.Null;
        }

        #endregion

    }

}

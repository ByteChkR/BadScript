using System.Text;

using BadScript.Runtime;
using BadScript.Runtime.Implementations;

namespace BadScript.Parser.Expressions.Value
{

    public class BSFunctionDefinitionExpression : BSExpression
    {

        public string Name;
        public bool Global;
        public string[] ArgNames;
        public BSExpression[] Block;

        #region Public

        public BSFunctionDefinitionExpression( string name, string[] args, BSExpression[] block, bool addGlobal )
        {
            Name = name;
            ArgNames = args;
            Block = block;
        }

        public static BSRuntimeObject InvokeBlockFunction(
            BSEngineScope scope,
            BSExpression[] block,
            string[] argNames,
            BSRuntimeObject[] arg )
        {
            for (int i = 0; i < arg.Length; i++)
            {
                scope.AddLocalVar(argNames[i], arg[i]);
            }

            foreach ( BSExpression buildScriptExpression in block )
            {
                buildScriptExpression.Execute( scope );

                if ( scope.ReturnValue != null )
                {
                    break;
                }
            }

            return scope.ReturnValue;
        }

        public override BSRuntimeObject Execute( BSEngineScope scope )
        {
            BSRuntimeFunction f =
                new BSRuntimeFunction( GetHeader(), x => InvokeBlockFunction( scope, x ) );

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

        #endregion

        #region Private

        private string GetHeader()
        {
            StringBuilder sb = new StringBuilder( $"function {Name}(" );

            for ( int i = 0; i < ArgNames.Length; i++ )
            {
                string argName = ArgNames[i];

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

        private BSRuntimeObject InvokeBlockFunction( BSEngineScope rootScope, BSRuntimeObject[] arg )
        {
            BSEngineScope funcScope = new BSEngineScope( rootScope );

            return InvokeBlockFunction( funcScope, Block,ArgNames, arg ) ?? new EngineRuntimeObject( null );
        }

        #endregion

    }

}

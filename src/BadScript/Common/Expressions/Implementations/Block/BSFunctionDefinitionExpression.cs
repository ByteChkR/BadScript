using System.Linq;
using System.Text;
using BadScript.Common.Exceptions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSFunctionDefinitionExpression : BSExpression
    {
        public string Name;
        public bool Global;
        public (bool, string)[] ArgNames;
        public BSExpression[] Block;

        #region Public

        public BSFunctionDefinitionExpression(
            string name,
            (bool, string)[] args,
            BSExpression[] block,
            bool addGlobal )
        {
            Name = name;
            ArgNames = args;
            Block = block;
            Global = addGlobal;
        }

        public BSFunctionDefinitionExpression( string name, string[] args, BSExpression[] block, bool addGlobal )
        {
            Name = name;
            ArgNames = args.Select( x => ( true, x ) ).ToArray();
            Block = block;
            Global = addGlobal;
        }

        public static ABSObject InvokeBlockFunction(
            BSScope scope,
            BSExpression[] block,
            string[] argNames,
            ABSObject[] arg )
        {
            return InvokeBlockFunction( scope, block, argNames.Select( x => ( true, x ) ).ToArray(), arg );
        }

        public static ABSObject InvokeBlockFunction(
            BSScope scope,
            BSExpression[] block,
            (bool, string)[] argNames,
            ABSObject[] arg )
        {
            for ( int i = 0; i < arg.Length; i++ )
            {
                ( bool allowNull, string name ) = argNames[i];

                if ( !allowNull && arg[i].IsNull )
                {
                    throw new BSRuntimeException( $"Parameter '{name}' can not be null" );
                }

                scope.AddLocalVar( name, arg[i] );
            }

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
            BSFunction f =
                new BSFunction( GetHeader(), x => InvokeBlockFunction( scope, x ), ArgNames.Length );

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

        #endregion

        #region Private

        private string GetHeader()
        {
            StringBuilder sb = new StringBuilder( $"function {Name}(" );

            for ( int i = 0; i < ArgNames.Length; i++ )
            {
                string argName = ArgNames[i].Item2;

                if ( !ArgNames[i].Item1 )
                {
                    argName = "!" + argName;
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

            return InvokeBlockFunction( funcScope, Block, ArgNames, arg ) ?? new BSObject( null );
        }

        #endregion
    }

}

﻿using System.Linq;
using System.Text;
using BadScript.Common.Exceptions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;

namespace BadScript.Common.Expressions.Implementations.Block
{

    public class BSFunctionParameter
    {
        public string Name;
        public bool IsOptional;
        public bool NotNull;

        public BSFunctionParameter( string name, bool notNull, bool optional )
        {
            Name = name;
            NotNull = notNull;
            IsOptional = optional;
        }
    }
    public class BSFunctionDefinitionExpression : BSExpression
    {
        public string Name;
        public bool Global;
        public BSFunctionParameter[] ArgNames;
        public BSExpression[] Block;

        public override bool IsConstant => false;

        #region Public

        public BSFunctionDefinitionExpression(
            SourcePosition srcPos,
            string name,
            BSFunctionParameter[] args,
            BSExpression[] block,
            bool addGlobal ) : base( srcPos )
        {
            Name = name;
            ArgNames = args;
            Block = block;
            Global = addGlobal;
        }

        //public BSFunctionDefinitionExpression(
        //    SourcePosition srcPos,
        //    string name,
        //    string[] args,
        //    BSExpression[] block,
        //    bool addGlobal ) : base( srcPos )
        //{
        //    Name = name;
        //    ArgNames = args.Select( x => ( true, x ) ).ToArray();
        //    Block = block;
        //    Global = addGlobal;
        //}

        

        public static ABSObject InvokeBlockFunction(
            BSScope scope,
            BSExpression[] block,
            BSFunctionParameter[] argNames,
            ABSObject[] arg )
        {
            for ( int i = 0; i < argNames.Length; i++ )
            {
                BSFunctionParameter p = argNames[i];
                if ( arg.Length <= i && !p.IsOptional)
                {
                    throw new BSRuntimeException("Missing Argument: " + p.Name);
                }
                
                if ( p.NotNull && (arg.Length<=i || arg[i].IsNull) )
                {
                    throw new BSRuntimeException( arg[i].Position, $"Parameter '{p.Name}' can not be null or is missing" );
                }

                scope.AddLocalVar( p.Name, arg.Length <= i?BSObject.Null : arg[i] );
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
                new BSFunction( GetHeader(), x => InvokeBlockFunction( scope, x ), ArgNames.Count(x=>!x.IsOptional), ArgNames.Length );

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
                string argName = ArgNames[i].Name;

                if ( ArgNames[i].NotNull )
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

            return InvokeBlockFunction( funcScope, Block, ArgNames, arg ) ?? BSObject.Null;
        }

        #endregion
    }

}

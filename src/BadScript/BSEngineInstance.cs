using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadScript.Common.Exceptions;
using BadScript.Common.Expressions;
using BadScript.Common.Runtime;
using BadScript.Common.Types;
using BadScript.Common.Types.Implementations;
using BadScript.Common.Types.References;

namespace BadScript
{

    public class BSEngineInstance
    {
        public readonly ABSTable GlobalTable;

        #region Public

        public BSEngineInstance( Dictionary < string, ABSObject > startObjects )
        {
            Dictionary < ABSObject, ABSObject > globalTable =
                new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, ABSObject > buildScriptEngineRuntimeObject in startObjects )
            {
                globalTable[new BSObject( buildScriptEngineRuntimeObject.Key )] =
                    buildScriptEngineRuntimeObject.Value;
            }

            GlobalTable = new BSTable( globalTable );

            GlobalTable.InsertElement( new BSObject( "__G" ), GlobalTable );

            GlobalTable.InsertElement(
                new BSObject( "loadString" ),
                new BSFunction( "function loadString(str)", LoadStringApi, 1 )
            );
        }

        public ABSObject LoadFile( string file, string[] args = null )
        {
            return LoadString( File.ReadAllText( file ), args );
        }

        public ABSObject LoadFile( string file, ABSObject[] args = null )
        {
            return LoadString( File.ReadAllText( file ), args );
        }

        public ABSObject LoadString( string script, string[] args = null )
        {
            return LoadString(
                script,
                args?.Select( x => ( ABSObject ) new BSObject( x ) ).ToArray()
            );
        }

        public ABSObject LoadString( string script, ABSObject[] args = null )
        {
            BSParser parser = new BSParser( script );
            BSExpression[] exprs = parser.ParseToEnd();

            BSScope scope = new BSScope( this );

            scope.AddLocalVar(
                "args",
                args == null
                    ? ( ABSObject ) new BSObject( null )
                    : new BSArray( args )
            );

            foreach ( BSExpression buildScriptExpression in exprs )
            {
                buildScriptExpression.Execute( scope );

                if ( scope.BreakExecution )
                {
                    break;
                }
            }

            return scope.Return ?? scope.GetLocals();
        }

        #endregion

        #region Private

        private ABSObject LoadStringApi( ABSObject[] arg )
        {
            ABSObject o = arg[0].ResolveReference();

            if ( o.TryConvertString( out string path ) )
            {
                ABSObject ret = LoadString( path, arg.Skip( 1 ).ToArray() );

                return ret;
            }

            throw new BSInvalidTypeException(
                "Expected String",
                o,
                "string"
            );
        }

        #endregion
    }

}

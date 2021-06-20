using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.Parser;
using BadScript.Parser.Expressions;
using BadScript.Runtime.Implementations;

namespace BadScript.Runtime
{

    public class BSEngineInstance
    {

        public readonly BSRuntimeTable GlobalTable;

        #region Public

        public BSEngineInstance( Dictionary < string, BSRuntimeObject > startObjects )
        {
            Dictionary < BSRuntimeObject, BSRuntimeObject > globalTable =
                new Dictionary < BSRuntimeObject, BSRuntimeObject >();

            foreach ( KeyValuePair < string, BSRuntimeObject > buildScriptEngineRuntimeObject in startObjects )
            {
                globalTable[new EngineRuntimeObject( buildScriptEngineRuntimeObject.Key )] =
                    buildScriptEngineRuntimeObject.Value;
            }

            GlobalTable = new EngineRuntimeTable( globalTable );

            GlobalTable.InsertElement( new EngineRuntimeObject( "__G" ), GlobalTable );

            GlobalTable.InsertElement(
                                      new EngineRuntimeObject( "loadString" ),
                                      new BSRuntimeFunction( "function loadString(str)", LoadStringApi )
                                     );
        }
        public BSRuntimeObject LoadFile(string file, string[] args = null)
        {
            return LoadString(File.ReadAllText(file), args);
        }

        public BSRuntimeObject LoadString( string script, string[] args = null )
        {
            return LoadString(
                              script,
                              args?.Select( x => ( BSRuntimeObject ) new EngineRuntimeObject( x ) ).ToArray()
                             );
        }

        public BSRuntimeObject LoadFile( string file, BSRuntimeObject[] args =null)
        {
            return LoadString( File.ReadAllText( file ), args );
        }

        public BSRuntimeObject LoadString( string script, BSRuntimeObject[] args=null)
        {
            BSParser parser = new BSParser( script );
            BSExpression[] exprs = parser.ParseToEnd();

            BSEngineScope scope = new BSEngineScope( this );

            scope.AddLocalVar(
                              "args",
                              args == null
                                  ? ( BSRuntimeObject ) new EngineRuntimeObject( null )
                                  : new EngineRuntimeArray( args )
                             );

            foreach ( BSExpression buildScriptExpression in exprs )
            {
                buildScriptExpression.Execute( scope );
            }

            return scope.ReturnValue ?? scope.GetLocals();
        }

        #endregion

        #region Private

        private BSRuntimeObject LoadStringApi( BSRuntimeObject[] arg )
        {
            BSRuntimeObject o = arg[0];

            if ( o is BSRuntimeReference r )
            {
                o = r.Get();
            }

            if ( o.TryConvertString( out string path ) )
            {
                BSRuntimeObject ret = LoadString( path, arg.Skip(1).ToArray() );

                return ret;
            }

            throw new Exception( "Expected String" );
        }

        #endregion

    }

}

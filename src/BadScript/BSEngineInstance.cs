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
        private readonly ABSTable m_StaticData;
        private readonly ABSTable m_GlobalTable;

        #region Public

        public BSEngineInstance( Dictionary < string, ABSObject > startObjects, Dictionary <string, ABSObject> gTable = null)
        {
            Dictionary < ABSObject, ABSObject > staticData =
                new Dictionary < ABSObject, ABSObject >();

            foreach ( KeyValuePair < string, ABSObject > buildScriptEngineRuntimeObject in startObjects )
            {
                staticData[new BSObject( buildScriptEngineRuntimeObject.Key )] =
                    buildScriptEngineRuntimeObject.Value;
            }

           BSTable sd = new BSTable( staticData );

           sd.InsertElement( new BSObject( "__G" ), sd);

           sd.InsertElement(
                new BSObject( "loadString" ),
                new BSFunction( "function loadString(str)", LoadStringApi, 1, int.MaxValue )
            );

           sd.Lock();
            m_StaticData = sd;

            if ( gTable == null )
            {
                m_GlobalTable = new BSTable();
            }
            else
            {
                Dictionary<ABSObject, ABSObject> globalTable =
                    new Dictionary<ABSObject, ABSObject>();
                foreach (KeyValuePair<string, ABSObject> buildScriptEngineRuntimeObject in gTable)
                {
                    globalTable[new BSObject(buildScriptEngineRuntimeObject.Key)] =
                        buildScriptEngineRuntimeObject.Value;
                }

                m_GlobalTable = new BSTable( globalTable );
            }
        }

        internal bool HasElement(ABSObject name)
        {
            return m_GlobalTable.HasElement(name) || m_StaticData.HasElement(name);
        }

        internal void InsertElement( ABSObject k, ABSObject v ) => m_GlobalTable.InsertElement( k, v );

        internal ABSObject GetElement(ABSObject name)
        {
            if (m_GlobalTable.HasElement(name))
                return m_GlobalTable.GetElement(name);
            if (m_StaticData.HasElement(name))
                return m_StaticData.GetElement(name);

            throw new BSRuntimeException( $"Can not Resolve name: '{name.SafeToString()}'" );
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

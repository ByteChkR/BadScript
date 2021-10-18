using System.IO;
using System.Linq;

using BadScript.ConsoleUtils;
using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.Scopes;

namespace BadScript.Testing
{

    public static class Program
    {

        #region Public

        public static void Main( string[] args )
        {
            BSEngineSettings es = BSEngineSettings.MakeDefault();
            es.Interfaces.Add( new BSCollectionInterface() );
            es.Interfaces.Add( new BSConvertInterface() );
            es.Interfaces.Add( new BSSystemConsoleInterface() );
            BSScope scope = es.BuildLocalEnvironment();

            BSEngine engine = scope.Engine;
            engine.LoadSource( File.ReadAllText( args[0] ), scope, args.Skip( 1 ).ToArray() );
        }

        #endregion

    }

}

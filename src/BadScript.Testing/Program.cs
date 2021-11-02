using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using BadScript.ConsoleUtils;
using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.Scopes;
using BadScript.Threading;

namespace BadScript.Testing
{

    public static class Program
    {

        public static void Test()
        {
            System.Console.WriteLine( "Hello World!" );
        }
        #region Public

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void Main( string[] args )
        {
            BSEngineSettings es = BSEngineSettings.MakeDefault();
            es.Interfaces.Add( new BSCollectionInterface() );
            es.Interfaces.Add( new BSConvertInterface() );
            es.Interfaces.Add( new BSSystemConsoleInterface() );
            es.Interfaces.Add(new BSThreadingInterface());


            BSEngine engine = es.Build();

            
            engine.LoadSource(File.ReadAllText(args[0]), args.Skip(1).ToArray());

        }

        #endregion

    }

}

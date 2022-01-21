using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BadScript.ConsoleUtils;
using BadScript.Interfaces.Collection;
using BadScript.Interfaces.Convert;
using BadScript.Parser.Expressions;
using BadScript.Parser.Expressions.Implementations.Binary;
using BadScript.Parser.Expressions.Implementations.Block;
using BadScript.Scopes;
using BadScript.StringUtils;
using BadScript.Threading;
using BadScript.Types;
using BadScript.Types.Implementations;

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
            es.Interfaces.Add( new BSThreadingInterface() );

            BSStringPlugin sp = new BSStringPlugin();
            sp.Load( es );

            BSEngine engine = es.Build();

            //Read, Parse and run File Contents
            engine.LoadFile( args[0] );

        }

        #endregion

    }

}

using System;
using System.Numerics;

using BadScript.ConsoleUtils;
using BadScript.Core;
using BadScript.Http;
using BadScript.HttpServer;
using BadScript.Imaging;
using BadScript.IO;
using BadScript.Json;
using BadScript.Math;
using BadScript.Process;
using BadScript.Settings;
using BadScript.StringUtils;
using BadScript.Utils;
using BadScript.Utils.Reflection;
using BadScript.Xml;
using BadScript.Zip;

namespace BadScript.Testing
{

    public static class Program
    {

        #region Public

        public static void Main( string[] args )
        {
            BSEngineSettings es = BSEngineSettings.MakeDefault();
            es.Interfaces.Add( new BadScriptCoreApi() );
            es.Interfaces.Add( new ConsoleApi() );
            es.Interfaces.Add( new ConsoleColorApi() );
            es.Interfaces.Add( new BS2JsonInterface() );
            es.Interfaces.Add( new Json2BSInterface() );
            es.Interfaces.Add( new BSFileSystemInterface() );
            es.Interfaces.Add( new BSFileSystemPathInterface( AppDomain.CurrentDomain.BaseDirectory ) );
            es.Interfaces.Add( new BSMathApi() );
            es.Interfaces.Add( new HttpApi() );
            es.Interfaces.Add( new HttpServerApi() );
            es.Interfaces.Add( new ProcessApi() );
            es.Interfaces.Add( new StringUtilsApi() );
            es.Interfaces.Add( new ZipApi() );
            es.Interfaces.Add( new ImagingApi() );
            es.Interfaces.Add( new VersionToolsInterface() );
            es.Interfaces.Add( new XmlInterface() );
            es.Interfaces.Add( BSReflectionInterface.Instance );
            
            BSReflectionInterface.Instance.AddType<Version>();
            BSReflectionInterface.Instance.AddType<DateTime>();
            BSReflectionInterface.Instance.AddType<TimeSpan>();
            BSReflectionInterface.Instance.AddType<Vector4>();
            

            BSEngine e = es.Build();
            e.LoadFile( "D:\\Users\\Tim\\Documents\\BadScript\\src\\BadScript.Testing\\ReflectionTest.bs" );
        }

        #endregion

    }

}

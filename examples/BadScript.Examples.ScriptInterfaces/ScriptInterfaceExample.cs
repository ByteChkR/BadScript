using System;
using System.Linq;

using BadScript.Interfaces;
using BadScript.Reflection;
using BadScript.Types;
using BadScript.Types.Implementations;

namespace BadScript.Examples.ScriptInterfaces
{

    public class MyCustomScriptInterface : ABSScriptInterface
    {

        public MyCustomScriptInterface(  ) : base("MyCustomInterface")
        {
        }

        public override void AddApi( ABSTable root )
        {
            root.InsertElement( "AssemblyName", typeof( MyCustomScriptInterface ).Assembly.FullName );

            root.InsertElement(
                               "GetLoadedAssemblies",
                               new BSFunction( "function GetLoadedAssemblies", GetLoadedAssemblies, 0 )
                              );

            //Because we are trying to set an ABSReference as value, we need to make sure that the table does not resolve the reference away when inserting it.
            root.SetRawElement(
                               "WindowName",
                               new BSReflectionReference(
                                                         () => new BSObject( Console.Title ),
                                                         x => Console.Title = x.ConvertString()
                                                        )
                              );
        }

        private ABSObject GetLoadedAssemblies( ABSObject[] arg )
        {
            return new BSArray(
                                      AppDomain.CurrentDomain.GetAssemblies().Select( x => new BSObject( x.FullName ) )
                                     );
        }

    }

    internal class ScriptInterfaceExample
    {
        private static BSEngine CreateEngine()
        {
            Console.WriteLine("Creating Script Engine");
            BSEngineSettings settings = BSEngineSettings.MakeDefault();

            settings.Interfaces.Add( new MyCustomScriptInterface() ); //Adding the custom script interface

            //Optional: Load MyCustomInterface on load. 
            //  settings.ActiveInterfaces.Add("MyCustomInterface");

            //Optional: Load MyCustomInterface into global table on load.
            //  settings.ActiveInterfaces.Add("#MyCustomInterface");


            return settings.Build();
        }


        private static void Main(string[] args)
        {
            string source = @"
                    Environment.LoadInterface(""MyCustomInterface"")
                    MyCustomInterface.WindowName = ""Script Interface Example""
                    return Environment.Debug(MyCustomInterface)
";

            BSEngine engine = CreateEngine();
            Console.WriteLine("Script Returned: \n" + engine.LoadSource( source ));
        }
    }
}

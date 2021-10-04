using System;

namespace BadScript.Examples.ScriptInterfaces
{

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

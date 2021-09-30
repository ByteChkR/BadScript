using System;

using BadScript.Common.Types;
using BadScript.Settings;

namespace BadScript.Examples.Integration
{

    internal class IntegrationExample
    {

        #region Private

        private static BSEngine CreateEngine()
        {
            Console.WriteLine( "Creating Script Engine" );
            BSEngineSettings settings = BSEngineSettings.MakeDefault();

            return settings.Build();
        }

        private static void Main( string[] args )
        {
            BSEngine engine = CreateEngine();

            //Using the GreeterWrapper and NameListWrapper
            string source = @"
                greeter = args[0]
                greetingList = args[1]

                foreach name in greetingList //Use for each on NameListWrapper(Implements IEnumerable<ForEachIteration>)
                {
                    greeter.Name = name //Set the Name
                    greeter.Greet() //Call Greet Function
                }
";

            Console.Write( "Enter Names(delimit by comma): " );
            string names = Console.ReadLine();
            string[] nameArray = names.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

            //Prepare Arguments
            ABSObject[] arguments =
            {
                new GreeterWrapper( new Greeter() ), //Add the wrapper object with a new instance of the GreetingObject
                new NameListWrapper( new NameList( nameArray ) ) //Add the Name List
            };

            engine.LoadSource( source, arguments );
        }

        #endregion

    }

}

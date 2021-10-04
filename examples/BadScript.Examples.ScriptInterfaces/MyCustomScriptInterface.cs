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

}
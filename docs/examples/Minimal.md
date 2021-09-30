# Bad Script Minimal Example

[Example Project](https://github.com/ByteChkR/BadScript/tree/develop/examples/BadScript.Examples.Minimal)

This example aims to be as small as possible.

## Creating a Script Engine Instance
```cs
private static BSEngine CreateEngine()
{
    Console.WriteLine( "Creating Script Engine" );
    //Use Default Settings.
    BSEngineSettings settings = BSEngineSettings.MakeDefault();

    //Build the Engine from the Settings
    return settings.Build();
}
```

## Running a Script
```cs
private static void Main( string[] args )
{
    BSEngine engine = CreateEngine();

    //The Source Code we want to execute
    string source = "return 92253/1337";

    Console.WriteLine( $"Source: {source}" );

    //Loading and Running the Source.
    //We save the return value in this case.
    //If the script does not return anything,
    //the returned Object will have the property "IsNull" set to true
    ABSObject returnValue = engine.LoadSource( source );

    Console.WriteLine( $"Output: {returnValue}" );
}
```

[List of all Example Projects](./Examples.md)
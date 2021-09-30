# Bad Script Console Example

[Example Project](https://github.com/ByteChkR/BadScript/tree/develop/examples/BadScript.Examples.Console)


A small program that has console output and can run scripts from files.

## Creating the Script Engine Instance
```cs
private static BSEngine CreateEngine()
{
    Console.WriteLine( "Creating Script Engine" );
    BSEngineSettings settings = BSEngineSettings.MakeDefault();

    //Add the Console API so we can write things to the console
    settings.Interfaces.Add( new ConsoleApi() );

    return settings.Build();
}
```

## The Console Code
```cs
private static void Main( string[] args )
{
    BSEngine engine = CreateEngine();

    if ( args.Length == 0 )
    {
        Console.WriteLine( "No File Specified" );

        return;
    }

    string script = args[0]; //The Script to Execute
    string[] scriptArgs = args.Skip( 1 ).ToArray(); //Skip Script Path for convenience

    //Run the Script from File
    engine.LoadFile( script, scriptArgs );
}
```

## Example Scripts

The example includes some example bad scripts

### `scripts/helloworld.bs`
Prints `Hello World` on the console.

Complete Console Output:
```
Creating Script Engine
Hello World

```

### `scripts/print_args.bs`
Prints all specified arguments to the console

Complete Console Output:
```
Creating Script Engine
Argument Count: 3
Argument0
Argument1
Argument2

```

### `scripts/print_globals.bs`
Prints all global variable names to the console

Complete Console Output:
```
Creating Script Engine
__G
print
write
read
clear
environment

```

[List of all Example Projects](./Examples.md)
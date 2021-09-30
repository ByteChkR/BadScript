# Bad Script Calculator Example

[Example Project](https://github.com/ByteChkR/BadScript/tree/master/examples/BadScript.Examples.Calculator)

This example is a small expression calculator.
Using a custom scope to keep variables between executions.


## The Calculator Script
```cs
private static void Main( string[] args )
{
    BSEngine engine = CreateEngine();
    
    //Create a Scope that we can use to run our expression in
    BSScope scope = new BSScope(engine); 
    while ( true )
    {
        Console.Write( "Enter Expression to Calculate: " );
        string expr = Console.ReadLine();

        try
        {
            //Parse the Input from string
            BSExpression[] expressions = engine.ParseString( expr );

            //Make sure that we only got one expression
            if ( expressions.Length != 1 )
            {
                Console.WriteLine( "Invalid Input. Got multiple Expressions" );
                continue;
            }

            //Execute the expression inside the scope we created
            ABSObject ret = expressions[0].Execute( scope );

            Console.WriteLine( $"Output: {expr} = {ret}" );
        }
        catch ( Exception e ) //In case anything goes wrong we gracefully try again.
        {
            Console.WriteLine( $"({e.GetType().Name})Input Error: {expr}" );
        }
    }
}
```

[List of all Example Projects](./Examples.md)
# Bad Script Integration Example

[Example Project](https://github.com/ByteChkR/BadScript/tree/master/examples/BadScript.Examples.Integration)

This example is a showcase on how one might integrate the language in their projects.

The example takes a greeter object and a list of names as input.
The Script will set the Greeter.Name field and call Greet() for each name.

In This Example:
- Implementing an `ABSObject` from scratch
- Implementing a `BSObject`(simplified way of doing it)
- Making it possible to use `foreach` with a custom object

## The Bad Script Source
```js
greeter = args[0]
greetingList = args[1]

foreach name in greetingList
{
    greeter.Name = name //Set the Name
    greeter.Greet() //Call Greet Function
}
```

## C# Type: Greeter
```cs
public class Greeter
{

    public string Name;

    #region Public

    public void Greet()
    {
        if ( string.IsNullOrEmpty( Name ) )
        {
            Console.WriteLine( "Hello User!" );
        }
        else
        {
            Console.WriteLine( $"Hello {Name}!" );
        }
    }

    #endregion

}
```

## C# Type: NameList
```cs
public class NameList
{

    public readonly string[] Names;

    #region Public

    public NameList( string[] names )
    {
        Names = names;
    }

    #endregion

}
```

## The Main Function
```cs
private static void Main( string[] args )
{
    BSEngine engine = CreateEngine();
    string source = @"...";

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
```

## Name List Wrapper
The Wrapper is meant to showcase the simplest way to creating an `ABSObject` by inheriting from `BSObject` which has all abstract members already declared with default implementations

```cs
public class NameListWrapper :
	    BSObject, 	//Override BSObject to have all members already overridden with their default implementations
	    IEnumerable < IForEachIteration > //Implement this interface, to be able to "foreach" on the object
{

    private readonly NameList m_List;

    #region Public

    public NameListWrapper( NameList list ) : base( list ) //Pass the Wrapped Instance to base
    {
        m_List = list;
    }

    public IEnumerator < IForEachIteration > GetEnumerator()
    {
        //Loop through the names and yield them
        foreach ( string listName in m_List.Names )
        {
            yield return new ForEachIteration( new BSObject( listName ) );
        }
    }

    #endregion

    #region Private

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

}
```


## Greeter Wrapper
This wrapper does implement all `ABSObject` functions itself.

```cs
public class GreeterWrapper : ABSObject
{

    private readonly Greeter m_Object;

    private readonly ABSReference m_Name;
    private readonly BSFunction m_GreetFunction;
    private readonly ABSReference m_GreetReference;

    //IsNull is returning true if the Greeter instance is null.
    public override bool IsNull => m_Object == null;

    #region Public

    public GreeterWrapper( Greeter obj ) : base( SourcePosition.Unknown )
    {
        m_Object = obj; //Store the reference to be able to check for equality

        //Creating the References using Custom References for Properties and Functions
        m_Name = new BSReflectionReference(
                                           () => new BSObject(
                                                              m_Object.Name
                                                             ), //Getter. Wrap the string into a BSObject
                                           value => m_Object.Name =
                                                        value.IsNull
                                                            ? null
                                                            : value.
                                                                ConvertString() //Setter. Convert and Assign Value.
                                          );
       	
       	//Create a BSFunction that can be executed
        m_GreetFunction = new BSFunction(
                                         "function Greet()", //Debug info
                                         args => //Arguments are not used here.
                                         {
                                             m_Object.Greet(); //Display Greeting

                                             return BSObject.Null;
                                         },
                                         0 //Argument Count
                                        );

        //Creating a Reference for the Function
        m_GreetReference = new BSFunctionReference( //Function Reference is readonly
                                                   m_GreetFunction
                                                  );
    }

    public override bool Equals( ABSObject other )
    {
    	//Implement somewhat standard equals logic.
        return other is GreeterWrapper ow && m_Object == ow.m_Object;
    }

    public override ABSReference GetProperty( string propertyName )
    {
    	//Manually Check if the propertyname is Name or Greet
    	//Returning the appropriate Reference if they match
        if ( propertyName == "Name" )
        {
            return m_Name;
        }

        if ( propertyName == "Greet" )
        {
            return m_GreetReference;
        }

        //Throwing here is good practise.
        //The Script Engine is not calling this function if the property does not exist.
        throw new BSRuntimeException( $"Can not find Property '{propertyName}'" );
    }


    public override bool HasProperty( string propertyName )
    {
        return propertyName == "Name" || propertyName == "Greet";
    }

    public override ABSObject Invoke( ABSObject[] args )
    {
    	//We can not invoke this object.
    	//So make it blow up if a script tries to do so.
        throw new BSRuntimeException( $"Can not Invoke Object '{this}'" );
    }

    public override string SafeToString( Dictionary < ABSObject, string > doneList )
    {
        //Unused in this Example.
        //Can be used to get a full list of properties
        return $"Greeting Wrapper Object: {m_Object}";
    }

    public override void SetProperty( string propertyName, ABSObject obj )
    {
        //Because the Greet Function is readonly and
        //the Name Property has its setter defined in the reference,
        //this function will never get called, so we can put a NotSupportedException
        throw new NotSupportedException( "Can not set properties on this object" );
    }

    public override bool TryConvertBool( out bool v )
    {
        //We do not allow any kind of conversion to base types
        v = false;

        return false;
    }

    public override bool TryConvertDecimal( out decimal d )
    {
        //We do not allow any kind of conversion to base types
        d = 0;

        return false;
    }

    public override bool TryConvertString( out string v )
    {
        //We do not allow any kind of conversion to base types
        v = null;

        return false;
    }

    #endregion

}
```

[List of all Example Projects](./Examples.md)
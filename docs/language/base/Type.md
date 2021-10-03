# Bad Script Typesystem
Bad Script supports the definition of types.
The simplest possible type one can define only has a name and a predefined set of [default functions](#default-functions).
```js
class MyType
{

}
```

## Default Functions

### GetType()
TODO

### ToString()
TODO

### IsInstanceOf(other)
TODO

### IsInstanceOf(otherName)
TODO


## Type Constructors
Types define an empty default constructor with no arguments if it has no implicit constructor implementations.
A type can only have a single constructor, which has to have the same name as it's enclosing type.
```js
class MyType
{
	function MyType() //constructor definition
	{

	}
}
```

Constructors can define parameters just like ordinary functions

```js
class MyType
{
	function MyType(x, y)
	{

	}
}
```

### Calling Constructors
Constructors are invoked when creating a type instance with the new keyword.
```js
instance = new MyType()
```

Although it is not recommended, the constructor can also be invoked on already existing instances

```js
instance = new MyType()
instance.MyType()
```

## this keyword
The this keyword can be used to qualify type members inside member functions.
This can be useful if function parameters hide members of the enclosing type
```js
class MyType
{
	x = 0 //define variable in type

	function MyType(value)
	{
		x = value // this is not required, as x is unambiguous
	}

	function SetX(x)
	{
		this.x = x // qualify MyType.x with the this keyword
	}
}
```

## Inheritance
Types can inherit from each other which makes the subtype also have all members of the base type.
```js
class MyBaseType
{
	X = 0
}

class MySubType : MyBaseType
{
	function DoStuffWithX()
	{
	return X * X
	}

}
```

In this example X can be used as if it got defined in the subclass.

### base Constructors
If a base class defines a constructor it can be invoked by 2 different methods

#### Base Invocation Syntax
The less error-prone way, using special function syntax to call base functions
```js
class MyBaseType
{
	X = 0
	function MyBaseType(x)
	{
		X = x
	}
}

class MySubType : MyBaseType
{
	function MySubType(x) : base(x)
	{
	}
}
```

### Explicit Constructor Invocation
By calling the constructor directly
```js
class MyBaseType
{
	X = 0
	function MyBaseType(x)
	{
		X = x
	}
}

class MySubType : MyBaseType
{
	function MySubType(x)
	{
		base.MyBaseType(x)
	}
}
```

## base keyword
As described in the [base Constructors section](#base-Constructors), the base Keyword can be used to call constructors.
Furthermore the base-keyword can be used to explicitly access properties and functions defined in the base class.
```js
class MyBaseType
{
	X = 0
	function MyBaseType(x)
	{
		X = x
	}
}

class MySubType : MyBaseType
{
	function MySubType(x)
	{
		base.X = x
	}
}
```
But it can also be used to call overridden functions (if they dont have a return or it is not used in the implementation)
```js
class MyBaseType
{
	X = 0
	function SetX(x)
	{
		X = x
	}

}

class MySubType : MyBaseType
{
	function SetX(x) : base(x * 2)
	{

	}
}
```

If the return value is used in the function implementation it is possible to call the function directly
```js
class MyBaseType
{
	X = 0
	function GetX()
	{
		return X
	}

}

class MySubType : MyBaseType
{
	function GetX()
	{
		return base.GetX() * 2
	}
}
```

## Namespaces
Namespaces can be used to hide types from the global scope.
```js
namespace MyNamespace
{

}
```

Namespaces were introduced to contain logic of a specific library or app without unintentionally hiding types of other implementations.
Namespaces can be defined inside another namespace, although this has no effect to it's relative position to the containing namespace. All namespace names are fully qualified.

## Using statements
Can be used to make types that are inside namespaces accessible to the current scope.

```js
namespace MyApp.Internals
{
	//Internal types that are only needed inside the app
}


namespace MyApp
{
	using MyApp.Internals

	//Do stuff with types from "MyApp.Internals"
}
```

[List of all Built-In Types](./BuiltInTypes.md)
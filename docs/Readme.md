# Bad Script Engine

## Base Functions
Base Functions are available by default in every execution environment and can be replaced by custom implementations.

### size(table/array)
returns the size of the object

### keys(table)
returns a new array of table keys

### values(table)
returns a new array of table values

### format(formatString, arg0, arg1, ...)
uses C# `string.Format`

### print(obj)
writes a line to the output stream


## Type System
The Base types are used to represent all objects that can be used inside the language.

### Base Type: ABSObject
Base Type for all Objects used inside the Script Engine

### Base Type: ABSArray
Inherits from `ABSObject`
Represents a List `ABSObjects` that is able to resize itself to accomodate for a dynamic count of items

### Base Type: ABSTable
Inherits from `ABSObject`
Represents a KeyValue Pair Collection of `ABSObjects`

### Implementations
Bad Script Comes with Base Implementations for each type of ABSObject

#### BSObject
Wrapper for C# Objects as strings and numbers

#### BSArray
Implements an Array using `System.Collections.Generic.List<ABSObject>` as internal implementation
`BSArray` implements special readonly functions:

- clear()
- size()
- add(obj)
- remove(obj)
- removeAt(index)

Operator Implementations:

- a[index]
- foreach value in a



#### BSTable
Implements a Table using `System.Collections.Generic.Dictionary<ABSObject, ABSObject>` as internal implementation

Operator Implementations:

- Access Values
	- t[key]
	- t.key
- foreach k in t
- foreach (k, v) in t

## Operators
List of Supported Operators

### add(+)

Adds two objects together
Example: `c = a + b`

#### Notes
Adding has multiple implementations based on the types of the objects that are added

`<object> + <object> = <string>`

`<number> + <number> = <number>`

`<number> + <object> = <string>`

`<object> + <number> = <string>`

### subtract(-)
### multiply(\*)
### divide(/)
### modulo(%)

### equal(==)
### unequal(!=)
### less than(<)
### greater than(>)
### less or equal(<=)
### greater or equal(>=)

### logical and(&&)
### logical or(||)
### logical xor(^)
### logical not(!)

### assignment(=)
### member access(.)

### nullcheck(??)
```js
a = null
notNull = a ?? 1
```

## Loops

### Keywords
all versions of for and while loops support both `break` and `continue`

### while
```js
x = 1
while(x == 1)
{

}
```

### for

#### Syntax
```js
for <var> = <start> while <end condition> step <expr>
{

}
```

#### Example
```js
for i = 0 while i < 10 step 1
{

}
```

#### Step Variable
Is not required. The Parser assumes `1`
```js
for i = 0 while i < 10
{

}
```

### for (short version)

#### Example
```js
for i = 0 while< 10
{

}
```
Equvalent End Condition without shortend syntax `i < 10`

#### Keywords

```
while= : operator ==
while! : operator !=
while< : operator <
while> : operator >
while<= : operator <=
while>= : operator >=
```

### foreach

#### Syntax
```js
foreach <value> in <collection>
{

}
```

#### Example
`foreach` loop on an array
```js
A = [] //Create new Table
A[0] = "Value1"
A[1] = "Value2"
foreach v in A
{

}
```

`foreach` loop on new array containing keys of table T
```js
T = {}
T["Key1"] = "Value1"
T["Key2"] = "Value2"
foreach key in keys(T)
{
	
}
```

`foreach` loop on the keys of the table, skips list allocation but does not allow removing of entries
```js
T = {}
T["Key1"] = "Value1"
T["Key2"] = "Value2"
foreach key in T
{
	
}
```

`foreach` loop on key and value pairs
```js
T = {}
T["Key1"] = "Value1"
T["Key2"] = "Value2"
foreach (key, value) in T
{
	
}
```

### Functions
#### Examples
Named Function
```js
function f(x, y)
{

}
```

Unnamed Function
```js
f = function(x, y)
{

}
```

#### Null Checked Parameters
Prefix a parameter with `!` to perform a null-check on the values for the parameters.
```js
function f(!x)
{

}

f(1) 
f(null) //Crash

```

#### Global Functions
Global Functions are made available at every scope level

```js
function myFunction()
{
	global function myGlobalFunction()
	{

	}
}
//Run the Function to define the confaining function.
//Until this function ran, myGlobalFunction is not defined
myFunction()


myGlobalFunction() //available from the outer scope because the definition is defined as global
```


#### Return Values

Functions always return null by default
```js
function f()
{

}

a = f()
```

Return a value
```js
function one()
{
	return 1
}
```

Return a value, use default return otherwise
```js
function uneven_or_null(x)
{
	if(x % 2 == 1)
	{
		return x
	}
}
```

#### Capturing Arguments in Lambda Functions
```js
function addValues(obj)
{
	return obj.x + obj.y
}

function makeObject()
{
	o = {}
	o.x = 10
	o.y = 10
	o.addValues = function()
	{
		return addValues(o)
	}
}

o = makeObject()
v1 = addValues(o) //Direct Use
v2 = o.addValues() //Use Lambda

```
# Bad Script Engine

## First Steps

### Download
- [Windows x64](https://bytechkr.github.io/BadScript/build-win.zip)
- [Linux x64](https://bytechkr.github.io/BadScript/build-linux.zip)

________

### Install on Linux

1. `mkdir ~/BadScriptEngine`
2. `cd ~/BadScriptEngine`
3. `wget https://bytechkr.github.io/BadScript/build-linux.zip`
4. `unzip build-linux.zip`
5. `chmod +x bs`

Optional Steps:

6. Make Alias to access `bs` anywhere by adding `alias bs="<home>/BadScriptEngine/bs"` to `~/.bashrc`
7. Reload Bash Environment Variables with `source ~/.bashrc`
8. Test Alias with `type -a bs`. It should print `bs is aliased to '/home/tim/BadScript/bs'`
________

### Install on Windows
1. Download Build and Extract

Optional Steps:

2. Add BS Shell Opener by writing a small powershell script with content. Usage: `PS> . <path/to/script>`
```powershell
# Path of the Install Directory(location of bs.exe)
$bs = "<Path of Install Directory>"

# Make Sure that the Path is fully qualified
$bs = (Get-Item $bs).fullname

# Add to Environment Path Variable, Only for this powershell session
$Env:Path += ";$bs" 
```

3. Add Second Powershell Script that Opens a new Powershell Window with the First Script as start command.
```powershell
# Create New Powershell Window and run the BS Shell Opener.
start PowerShell -ArgumentList "-noexit -command . <path/to/first/script>"
```

### Usage
- Running Scripts: `bs <path/to/script>`
- Running Apps: `bs <appname/path>`

________

Notes:

The file extension is optional if the path points to a `*.bs` file(e.g: `bs main` is the same as `bs main.bs`).

Apps are stored `./bs-data/apps/` and can be called by passing the relative path of the app based on `./bs-data/apps/`.

Scripts are able to hide apps, `bs myApp` will execute `myApp.bs` in the current directory if it exists, otherwise it will try to execute app `./bs-data/apps/myApp.bs`

________


### Setup
The runtime comes with a default app called `apps-install` that can install other apps from repositories.

A good first step is to update the `apps-install` script with the command `bs apps-install apps-install`

After the script has been updated, other apps can be installed([Default App List](https://byt3.dev:3785/list)).

A small tool to manage the BS Runtime and its files is called `cliconf` and is recommended to be installed.

________

Notes:

The app `interactive` is very useful when prototyping or testing.(Install with `bs apps-install interactive`)

________


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

### nullchecked property(?.)
```js
a = {}
value = a?.propertyName //Null if a does not have property "propertyName". Otherwise its the value of "a.propertyName"
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
A = [] //Create new Array
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

#### Capturing Arguments in Functions
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



#### Single Expression Functions
Functions can also be declared in a similar syntax to C# lambdas
```js
function addValues(obj) => return obj.x + obj.y

function makeObject()
{
	o = {}
	o.x = 10
	o.y = 10
	o.addValues = function() => return addValues(o)
}

o = makeObject()
v1 = addValues(o) //Direct Use
v2 = o.addValues() //Use Lambda

```


# Function
Contains a Function that can be executed.


```js
function f()
{
	//Do Stuff
}
```

## `global`-Modifier
Functions that have the `global` modifier are made available at all scopes

```js
global function f()
{
	//Do Stuff
}
```

## Lambda Functions
Functions that only contain a single line, can be written in the `=>` format.

```js
function f(x) => return x * x
```

## Anonymous Functions
Functions can be declared without specifing a name.
Anonymous functions can not be `global`

```js
f = function(x)
{
	return x * x
}
```

```js
f = function(x) => return x * x
```

## Parameters
Functions can have a variable amount of parameters.
Parameters are only valid inside the declaring function.

```js
function f(x)
{
	//Do Stuff with x
}
```

### Null Checked Parameters
If a parameter is prefixed with `!` in the function signature, the Script Engine is guaranteeing that the function can not be invoked with that parameter beeing `null`

```js
function(!x)
{
	//Do Stuff with x
}

f(1)
f("AAA")
f(null) //Crash
```

### Optional Parameters
If a parameter is prefixed with `?` the function can be invoked without specifying the parameter. If the parameter is not specified, the parameter will contain `null`.

```js
function f(?x)
{
	if(x == null)
	{
		//Do Stuff without x
	}
	else
	{
		//Do Stuff with x
	}
}

f()
f(2)
```

### Array Parameters
If a function is defined with a single parameter which is prefixed with `*`, the Script Engine will pass an array of the arguments to the function, which makes the function accept all numbers of parameters.

```js
function f(*args)
{
	foreach arg in args
	{
		//Do Stuff with Argument
	}
}

f(1, "Hello World")
f(true)
f()

```

## Returning Values
When functions return values, the function needs to return a value in every possible execution path.
```js
function f() //Legal
{
	return "Hello World"
}

function f(x) //Legal
{
	if(x == true)
	{
		return "Hello World"
	}

	return null
}

function f(x) //Legal
{
	if(x == true)
	{
		return "Hello World"
	}
	else
	{
		return null
	}
}

function f(x) //Illegal
{
	if(x == true)
	{
		return "Hello World"
	}

	//Not all Code Paths return a value
}
```

## Default Operators

- [op_Equality(==)](./operators/Equality.md)
- [op_Inequality(!=)](./operators/Inequality.md)

## Implemented Functions

### invoke
Invokes the Function

```js
function f(x)
{
	return x * 2
}

v = 1
vDouble = f.invoke([v], true) 		//[v] = Defining an Array with one element(v)
									//true = Execute Hooks

vQuadruple = f.invoke([vDouble]) 	//[v] = Defining an Array with one element(v)
```

#### Required Parameters
- args
	- The Arguments of the Invocation

#### Optional Parameters
- execHooks(default: `true`)
	- if `false`, the function will be executed without any hooks

### hook
Hooks into a function. Every time this function gets invoked, all registered hooks are called.

```js
function f(x)
{
	return x * x
}

function NotifyIfInvoked(x)
{
	//Do Stuff
}

f.hook(NotifyIfInvoked) // Hooks NotifyIfInvoked into f

f(2) 					// NotifyIfInvoked gets called before f
```

```js
function f(x)
{
	return x * x
}

function InterceptIfInvoked(x)
{
	return x * x * x	// Returning anything else than null will abort the invocation
						// of all following hooks and the base function itself
}

f.hook(InterceptIfInvoked) //Hooks InterceptIfInvoked into f

f(2) 					// InterceptIfInvoked gets called before f. The return is 8
f.invoke([2], false) 	// InterceptIfInvoked does not get called. The return is 4
```

#### Required Parameters
- hookFunc
	- The Function that will be hooked.

### releaseHook
Releases the Specified hook from the function

```js
function f() {}
function f_hook() {}

f.hook(f_hook)
f() // Calls f_hook
f.releaseHook(f_hook)
f() // Does not call f_hook anymore
```

#### Required Parameters
- hookFunc
	- The Function that will be released.

### releaseHooks
Releases all hooks from the function

```js
function f() {}
function f_hook1() {}
function f_hook2() {}
f.hook(f_hook1)
f.hook(f_hook2)

f.releaseHooks() //Clears all Hooks
```
# Functions
### Examples
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

### Null Checked Parameters
Prefix a parameter with `!` to perform a null-check on the values for the parameters.
```js
function f(!x)
{

}

f(1) 
f(null) //Crash

```

### Global Functions
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


### Return Values

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

### Capturing Arguments in Functions
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



### Single Expression Functions
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

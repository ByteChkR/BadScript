# Bad Script Coding Best Practices

When writing BS Files there are some Practices that improve code readability and prevent hard to find bugs. This document tries to explain the concepts I use(and do not use) to write cleaner code.

## Keep things Local

Because Bad Script has no concept of private variables and functions, all functions and variables are available in all sub scopes of the script. Which is useful but also can result in many variables beeing accessible in some scopes.

The runtime allows to hide parent variables inside functions with parameters.

```js
a = 1
function f(a)
{
    a = 2 // modifies the parameter a.
}
f(20)
```
In this example the outer scope variable a is still 1 after running the function, because a is defined as function parameter which will hide the outer scope variable a.

```js
a = 1
function f()
{
    a = 2
}
f()
```
In this example the variable a is only defined in the outer scope and because of that, the variable outer a will be 2 after the function ran.

```js
function f()
{
    a = 2
}

f()
```
Here the variable is not defined in the outer scope and thus will remain undefined in the outer scope even after the function executed.

Because of this implementation detail, it is advised to be careful when declaring variables in the outer scope to prevent functions accidentally overwriting outer variables.

## Deleting Functions is dangerous
Because functions are (by design) nothing more than variables, it is possible to replace or delete those functions by intent or by accident.

```js
function f()
{
    //Do Some Stuff
}

f = null //Delete the function
f() // Is now undefined and will crash the program(unless it is defined in a outer scope.)
```

Good Advice is to chose different naming for functions to make it harder to overwrite functions by accident.
If dealing with functions inside tables which content is not meant to be assignable, it is possible to lock a complete table or array from beeing modified(this does only work for the table and its element. The element of an element will remain assignable)

```js
t = {
    MyFunction = function()
    {
        // Do Stuff
    },
    subTable = {
        a = 0
    }
}
Collection.Lock(t) //Requires the "Collection" interface to be available and loaded

t.MyFunction = null //Will now crash the runtime.
t.subTable.a = null //will still work

```

## Local Variable Naming Schemes
It is advised to prefix variables that are not meant to be publicly accessible with underscores, to make it clear.

```js
t = {
    _a = 0, //Using Private Variable naming scheme
    Value = 0,
    Func = function()
    {
        return t._a * t.Value
    }
}
```

With a trick it is possible to entirely make the `_a` variable hidden by using a function to capture the variable
```js
function createT()
{
    _a = 0 //A is defined in function scope and will become unavailable to all outer scopes.
    t = {
        Value = 0,
        Func = function()
        {
            return _a * t.Value //_a is accessible inside all sub scopes of the outer function.
        }
    }
    return t
}

t = createT()
//t has now only 2 properties which are accessible.
//yet, the function Func is still able to access `_a`.

```

## String Computations
Working with strings is very memory intensive as it is not possible to directly modify a string object.
When concatenating strings with `+` or `+=` there will always be a new string object created.

Therefore it is advised to keep string concatenations to a minimum to avoid unnessecary slowdowns

## Function Hooks exist. Dont use them.
While hooking a function is a very powerful feature, it has an incredible range of possible hard to detect bugs.
There is no limitation on who can hook functions.

### Stack Overflows
```js
function f()
{

}

f.Hook(f) //Will result in stack overflow.
```
```js
function f()
{

}

function f_Hook()
{
    f() //Will Stack overflow
}
f.Hook(f_Hook) //Will result in stack overflow.
```
There is however a way to invoke a function without calling the hooks
```js
function f()
{

}

function f_Hook()
{
    f.Invoke([], false) // Will execute f without executing all hooks again.
}
f.Hook(f_Hook)
```

### Function Hook Return Quirks
Function Hooks have special behaviour when returning a value.
If the returned value is not `null`, the runtime will abort all successive calls to hooked functions and abort the invocation of the hooked function.

This enables changing behaviour of functions after they have been written.
This is useful for debugging. For example logging all function calls and parameters, or modifying return values to make testing easier.
```js
function getPath()
{
    return "./MyPath/To/Release/File"
}

function getPath_DebugHook()
{
    //Potential Log that "getPath" was called.
    return "./MyPath/To/Debug/File"
}

getPath.Hook(getPath_DebugHook) //Hook the getPath Function for testing purposes

str = getPath() //Str will contain "Hook Executed", f() never ran.
```

### Function Hook Parameters
It is possible to hook functions that have parameters, but the user needs to make sure that the function signature is the same across all hooks and the hooked functions
```js
function f(x)
{
    return x * x
}

function f_Hook1(x)
{
    Console.WriteLine("X: " + x)
}

function f_Hook2(x, y)
{

}

f.Hook(f_Hook1)
f.Hook(f_Hook2) //Will work.
f(1) //Will crash because the runtime tries to invoke f_Hook2 with 1 arguments.
```

A way to workaround this limitation is to use function parameter modifiers.
```js
function f(x)
{
    return x * x
}

function f_Hook1(x, ?y)
{
    Console.WriteLine("X: " + x)
}

function f_Hook2(*args)
{
    foreach arg in args
    {
        Console.WriteLine("Arg: " + arg)
    }
}

f.Hook(f_Hook1)
f.Hook(f_Hook2)
f(1) //Will work correctly as both function signatures are valid for the input arguments provided.
```
But Parameter Modifiers can also result in crashes if used in an incompatible way.
```js
function f(x)
{
    return x * x
}

function f_Hook1(x)
{
    Console.WriteLine("X: " + x)
}

function f_Hook2(!x)
{

}

f.Hook(f_Hook1)
f.Hook(f_Hook2) //Will work.
f(null) //Will crash because the runtime tries to invoke f_Hook2 with null as first argument, 
        //which is explicitly not allowed in the function signature.
```

## Capturing Variables in local scopes
It is possible to capture variables into a scope that is completely unavailable to anthing else than its subscopes.
A useful application is demonstrated in Section [Local Variable Naming Schemes](#local-variable-naming-schemes) by using a function.
```js
function makeObject()
{
    constant = 5
    ret = {
        MyPublicFunction = function(x) => return x * constant + ret.offset,
        offset = 10
    }
    return ret
}

obj = makeObject()
obj.offset = 100 //This variable can still be modified
//while the 'constant' variable is completely inaccessible to anything else than the table itself and the createObject function.

```

## Types and 'this'/'base'
When defining types that do not have a base class, it `base` is not defined.

```js
class MyClass
{
    X = 0
    Y = 0
    function MyClass(x, y) //Defining a constructor with 2 parameters
    {
        X = x
        Y = y
        //X and Y can also be accessed with 'this.X' and 'this.Y'
    }

    function AddComponents()
    {
        return X + Y
    }

    function Set(x, y)
    {
        X = x
        Y = y
    }
}

class MySubClass : MyClass
{
    Z = 0
    //Calling base class constructors can be done in multiple ways.
    function MySubClass(x, y, z) : base(x, y) //Via base invocation syntax
    {
        //Or calling the function directly
        base.MyClass(x, y)
        Z = z
    }

    //Using base Syntax is also possible to do on non constructor classes.
    function AddComponents()
    {
        return base.AddComponents() + Z
    }

    functions SqrMagnitude()
    {
        return base.X * this.Y * Z //Base properties and variables can be accessed as if they are defined in the current class.
    }

    //Base invocations also work for non constructor functions when the base function does not return any values.
    function Set(x, y, z) : base(x, y) //Calls base.Set(x, y)
    {
        Z = z
    }
}
```

## Minimize Script Loads
When working on a bigger project it is often very useful to split the code across multiple files.
Executing the code then becomes a lot harder as you need to respect loading order and make sure that you loaded all files before jumping into the actual application code.

This can be worked around by using the `project make` system that will build a script file from multiple sources.

To Create a project from a template file use the command `project new -t <templatename>`.
The Console will then initialize a new project in the current working directory based on the template selected.

___

[Advanced Tricks](./AdvancedTricks.md)
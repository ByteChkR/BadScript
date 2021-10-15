# Bad Script Advanced Tricks
If trying to implement a complicated task it is often required to get a bit more creative to get to a working state.

## Generating Code at runtime
For projects that require custom logic that can not be implemented in advance, the `Environment` script interface can be helpful to execute generated code.
```js

source = "return args[0] * args[1] + args[2]"

ret = Environment.LoadScript(source, [1, 2, 3]) //Call Code that has not been parsed at script load.

```

### Executing Code in special scopes
Sometimes it is useful to encapsulate script executions into subscopes for persistency or to keep the current scope clean
```js
source = "return a * b + c"

subScope = Environment.CreateScope(__SELF) //Use the Default Variable containing the current scope as parent.

//Defining the Variables here means they will be accessible in the subScope.
a = 1
b = 2
c = 3

ret = Environment.LoadScopedString(subScope, source)
```

### Executing Code in same scope
Most of the times this is a very bad practise as you can seriously mess up the current scope by code that does not even exist until it is executed. Nevertheless it is a very powerful tool if used correctly.
```js
source = "result = a * b + c"

//Defining the Variables here
a = 1
b = 2
c = 3

Environment.LoadScopedString(__SELF, source) //Using __SELF as scope

myValue = result //Result is defined in the source we executed in the current scope.

```
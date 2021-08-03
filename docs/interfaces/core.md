# Core Interface

Provides Default Functions and Language Specific Features

## size(table/array)
returns the size of the object

## keys(table)
returns a new array of table keys

## values(table)
returns a new array of table values

## error(obj)
throws a `BSRuntimeException` with the specified message.

## debug(obj)
Returns a string representation of the passed object that is guaranteed to not crash.

## isArray(obj)
Returns true if the object is an array

## lock(array/table)
Locks the specified array/table to be readonly. (This operation can not be reversed.)

## isTable(obj)
Returns true if the object is a table

## hasKey(table, key)
Returns true if the table contains a property with the specified key

## escape(str)
Escapes a string sequence with C# Function `Uri.EscapeDataString`

## base64.from(str)
returns an array of numbers from a base64 encoded string

## base64.to(array)
encodes the numbers inside the array into a base64 string

## sleep(ms)
pauses the execution for `ms` milliseconds

## hook(target, hook)
Hooks the target function with the passed hook
Whenever the `target` gets invoked, the hook is executed and can return either `null`(continue with target) or a value(target is not executed and return from hook is used)

## releaseHook(target, hook)
Removes an active hook from the target

## releaseHooks(target)
Removes all hooks from the target

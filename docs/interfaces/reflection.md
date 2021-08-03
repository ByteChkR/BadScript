# Reflection Interface

Interface Key: `reflection`

Provides Functionality to use C# Reflection inside a BS Script

## Objects

### C# Reflection Object

Contains all properties that exist in the wrapped C# Object

## Functions

### getType(fullName)
returns the C# Type with the specified name

### loadAssembly(fullName)
makes sure that all types from the specifed assembly have been loaded.

### wrapInstance(instance)
Wraps an existing `C# Type Instance` into a Reflection Wrapper

### getConstructorData()
Returns a list of constructors of types that have been loaded and can be created
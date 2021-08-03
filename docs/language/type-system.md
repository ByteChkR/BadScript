# Type System
The Base types are used to represent all objects that can be used inside the language.

## Base Type: ABSObject
Base Type for all Objects used inside the Script Engine

## Base Type: ABSArray
Inherits from `ABSObject`
Represents a List `ABSObjects` that is able to resize itself to accomodate for a dynamic count of items

## Base Type: ABSTable
Inherits from `ABSObject`
Represents a KeyValue Pair Collection of `ABSObjects`

## Implementations
Bad Script Comes with Base Implementations for each type of ABSObject

### BSObject
Wrapper for C# Objects as strings and numbers

### BSArray
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



### BSTable
Implements a Table using `System.Collections.Generic.Dictionary<ABSObject, ABSObject>` as internal implementation

Operator Implementations:

- Access Values
	- t[key]
	- t.key
- foreach k in t
- foreach (k, v) in t
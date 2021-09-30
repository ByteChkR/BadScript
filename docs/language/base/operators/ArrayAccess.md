# Array Access Operator([])

Accesses an element inside an object by using an indexer

- Key: `[]`
- Function Signature: `op_ArrayAccess(other)`

```js
table = {
	a = true,
	op_ArrayAccess = function(other)
	{
		if(other == "a" || other == 0)
		{
			return table.a
		}
		return null
	}
}

b1 = table["a"] //returns value of a
b1 = table[0] //returns value of a
```

[List of All Operators](./Operators.md)
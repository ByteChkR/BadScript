# Member Access Operator(.)

Accesses an element inside an object by using a member name

- Key: `.`
- Function Signature: `op_MemberAccess(other)`

```js
table = {
	a = true,
	op_MemberAccess = function(other)
	{
		if(other == "a" || other == "b")
		{
			return table.a
		}
		return null
	}
}

b1 = table.a //returns value of a
b1 = table.b //returns value of a
```

[List of All Operators](./Operators.md)
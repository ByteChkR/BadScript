# Logical Not Operator(!)

Returns True if left and right are true

- Key: `!`
- Function Signature: `op_LogicalNot(other)`

```js
table = {
	a = true,
	op_LogicalNot = function(other)
	{
		return {
			a = !a
		}
	}
}

b1 = !table //b1.a == false
```

[List of All Operators](./Operators.md)
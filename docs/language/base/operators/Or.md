# Logical Or Operator(||)

Returns True if any of left or right are true

- Key: `||`
- Function Signature: `op_Or(other)`

```js
table = {
	a = true,
	op_Or = function(other)
	{
		return table.a || other.a
	}
}

o = {
	a = false,
	op_Or = function(other)
	{
		return o.a || other.a
	}
}
b1 = table || other //Returns true
```

[List of All Operators](./Operators.md)
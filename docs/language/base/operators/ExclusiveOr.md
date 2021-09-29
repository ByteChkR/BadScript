# Logical Exclusive Or Operator(^)

Returns True if either left or right are true

- Key: `^`
- Function Signature: `op_ExclusiveOr(other)`

```js
table = {
	a = true,
	op_ExclusiveOr = function(other)
	{
		return table.a ^ other.a
	}
}

o = {
	a = false,
	op_ExclusiveOr = function(other)
	{
		return o.a ^ other.a
	}
}
b1 = table ^ other //Returns true
```

[List of All Operators](./Operators.md)
# Logical Self Exclusive Or Operator(^=)

Returns True if either left or right are true

- Key: `^=`
- Function Signature: `op_SelfExclusiveOr(other)`

```js
table = {
	a = true,
	op_SelfExclusiveOr = function(other)
	{
		table.a ^= other.a
		return table
	}
}

o = {
	a = false,
	op_SelfExclusiveOr = function(other)
	{
		o.a ^= other.a
		return o
	}
}
table ^= other //Returns true
```

[List of All Operators](./Operators.md)
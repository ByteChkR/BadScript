# Logical Self Or Operator(|=)

Returns True if any of left or right are true

- Key: `|=`
- Function Signature: `op_SelfOr(other)`

```js
table = {
	a = true,
	op_SelfOr = function(other)
	{
		table.a |= other.a
		return a
	}
}

o = {
	a = false,
	op_SelfOr = function(other)
	{
		o.a |= other.a
		return o
	}
}
table |= other //table.a == true
```

[List of All Operators](./Operators.md)
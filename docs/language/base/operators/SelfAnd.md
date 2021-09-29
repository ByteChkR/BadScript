# Logical Self And Operator(&=)

Returns True if left and right are true

- Key: `&=`
- Function Signature: `op_SelfAnd(other)`

```js
table = {
	a = true,
	op_SelfAnd = function(other)
	{
		table.a &= other.a
		return table
	}
}

o = {
	a = true,
	op_SelfAnd = function(other)
	{
		o.a &= other.a
		return o
	}
}
table &= other //table.a == true
```

[List of All Operators](./Operators.md)
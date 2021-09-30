# Logical And Operator(&&)

Returns True if left and right are true

- Key: `&&`
- Function Signature: `op_And(other)`

```js
table = {
	a = true,
	op_And = function(other)
	{
		return table.a && other.a
	}
}

o = {
	a = true,
	op_And = function(other)
	{
		return o.a && other.a
	}
}
b1 = table && other //Returns true
```

[List of All Operators](./Operators.md)
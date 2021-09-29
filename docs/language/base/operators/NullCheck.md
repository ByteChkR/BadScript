# NullCheck Operator(??)

Returns the left side if not null. Otherwise returning right side

- Key: `??`
- Function Signature: `op_NullCheck(other)`

```js
table = {
	a = 10,
	op_NullCheck = function(other)
	{
		if(table.a == null)
		{
			return other
		}
		return table
	}
}

o = {
	a = null,
	op_NullCheck = function(other)
	{
		if(o.a == null)
		{
			return other
		}
		return o
	}
}
b1 = table ?? other //Returns table
```

[List of All Operators](./Operators.md)
# Self Division Operator(/=)

Divides the left side by the right side

- Key: `/=`
- Function Signature: `op_SelfDivision(other)`

```js
table = {
	a = 10,
	op_SelfDivision = function(other)
	{
		table.a = table.a / other.a
		return table
	}
}

o = {
	a = 2,
	op_SelfDivision = function(other)
	{
		o.a = o.a / other.a
		return o
	}
}
table /= other //table.a == 5
```

[List of All Operators](./Operators.md)
# Self Multiply Operator(\*=)

Multiplies the left side with the right side

- Key: `*=`
- Function Signature: `op_SelfMultiply(other)`

```js
table = {
	a = 10,
	op_SelfMultiply = function(other)
	{
		table.a = table.a * other.a
		return table
	}
}

o = {
	a = 9,
	op_SelfMultiply = function(other)
	{
		o.a = o.a * other.a
		return o
	}
}
table *= other //table.a == 90
```

[List of All Operators](./Operators.md)
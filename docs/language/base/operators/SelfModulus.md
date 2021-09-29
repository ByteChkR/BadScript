# Self Modulus Operator(%=)

Computes the remainder of division left / right

- Key: `%=`
- Function Signature: `op_SelfModulus(other)`

```js
table = {
	a = 9,
	op_SelfModulus = function(other)
	{
		table.a = table.a % other.a
		return table
	}
}

o = {
	a = 5,
	op_SelfModulus = function(other)
	{
		o.a = o.a % other.a
		return o
	}
}
table %= other //table.a == 4
```

[List of All Operators](./Operators.md)
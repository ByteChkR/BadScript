# Modulus Operator(%)

Returns the remainder of division left / right

- Key: `%`
- Function Signature: `op_Modulus(other)`

```js
table = {
	a = 9,
	op_Modulus = function(other)
	{
		r = {
			a = table.a % other.a
		}
		return r
	}
}

o = {
	a = 5,
	op_Modulus = function(other)
	{
		r = {
			a = o.a % other.a
		}
		return r
	}
}
b1 = table % other //Returns new table with a = 4
```

[List of All Operators](./Operators.md)
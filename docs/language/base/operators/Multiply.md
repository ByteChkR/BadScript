# Multiply Operator(\*)

Multiplies two objects with another

- Key: `*`
- Function Signature: `op_Multiply(other)`

```js
table = {
	a = 10,
	op_Multiply = function(other)
	{
		r = {
			a = table.a * other.a
		}
		return r
	}
}

o = {
	a = 9,
	op_Multiply = function(other)
	{
		r = {
			a = o.a * other.a
		}
		return r
	}
}
b1 = table * other //Returns new table with a = 90
```

[List of All Operators](./Operators.md)
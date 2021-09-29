# Subtraction Operator(-)

Subtracts two objects from another

- Key: `-`
- Function Signature: `op_Subtraction(other)`

```js
table = {
	a = 10,
	op_Subtraction = function(other)
	{
		r = {
			a = table.a - other.a
		}
		return r
	}
}

o = {
	a = 9,
	op_Subtraction = function(other)
	{
		r = {
			a = o.a - other.a
		}
		return r
	}
}
b1 = table - other //Returns new table with a = 1
```

[List of All Operators](./Operators.md)
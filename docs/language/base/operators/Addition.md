# Addition Operator(+)

Adds together two objects

- Key: `+`
- Function Signature: `op_Addition(other)`

```js
table = {
	a = 10,
	op_Addition = function(other)
	{
		r = {
			a = table.a + other.a
		}
		return r
	}
}

o = {
	a = 9,
	op_Addition = function(other)
	{
		r = {
			a = o.a + other.a
		}
		return r
	}
}
b1 = table + other //Returns new table with a = 19
```

[List of All Operators](./Operators.md)
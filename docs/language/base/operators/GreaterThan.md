# Greater Than Or Equal Operator(>)

Determines if the left object is greater than the right object.

- Key: `>`
- Function Signature: `op_GreaterThan(other)`

```js
table = {
	a = 10,
	op_GreaterThan = function(other)
	{
		return table.a > other.a
	}
}

o = {
	a = 9,
	op_GreaterThan = function(other)
	{
		return o.a > other.a
	}
}
b1 = table > other //Returns true
b2 = other > table //Returns false
```

[List of All Operators](./Operators.md)
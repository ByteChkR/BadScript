# Greater Than Or Equal Operator(<=)

Determines if the left object is less than the right object.

- Key: `<=`
- Function Signature: `op_LessThanOrEqual(other)`

```js
table = {
	a = 10,
	op_LessThanOrEqual = function(other)
	{
		return table.a <= other.a
	}
}

o = {
	a = 9,
	op_LessThanOrEqual = function(other)
	{
		return o.a <= other.a
	}
}
b1 = table <= other //Returns false
b2 = other <= table //Returns true
```

[List of All Operators](./Operators.md)
# Division Operator(/)

Divides the left side by the right side

- Key: `/`
- Function Signature: `op_Division(other)`

```js
table = {
	a = 10,
	op_Division = function(other)
	{
		r = {
			a = table.a / other.a
		}
		return r
	}
}

o = {
	a = 2,
	op_Division = function(other)
	{
		r = {
			a = o.a / other.a
		}
		return r
	}
}
b1 = table / other //Returns new table with a = 5
```

[List of All Operators](./Operators.md)
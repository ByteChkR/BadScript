# Equality Operator(==)

Determines if two Objects are equal.


- Key: `==`
- Function Signature: `op_Equality(other)`

```js
table = {
	a = 10,
	op_Equality = function(other)
	{
		return other.a == table.a
	}
}

o = {
	a = 10,
	op_Equality = function(other)
	{
		return other.a == o.a
	}
}
b1 = table == other //Returns true
b2 = other == table //Returns true
```

[List of All Operators](./Operators.md)
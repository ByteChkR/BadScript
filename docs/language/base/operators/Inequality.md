# Inequality Operator(!=)

Determines if two Objects are inequal.


- Key: `==`
- Function Signature: `op_Inequality(other)`

```js
table = {
	a = 10,
	op_Inequality = function(other)
	{
		return other.a != table.a
	}
}

o = {
	a = 9,
	op_Inequality = function(other)
	{
		return other.a != o.a
	}
}
b1 = table != other //Returns true
b2 = other != table //Returns true
```

[List of All Operators](./Operators.md)
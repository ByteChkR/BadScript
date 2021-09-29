# Self Addition Operator(+=)

Subtracts two objects from another

- Key: `+=`
- Function Signature: `op_SelfAddition(other)`

```js
table = {
	a = 10,
	op_SelfAddition = function(other)
	{
		table.a = table.a + other.a
		return table
	}
}

o = {
	a = 9,
	op_SelfAddition = function(other)
	{
		o.a = o.a + other.a
		return o
	}
}
table += other //table.a == 19
```

[List of All Operators](./Operators.md)
# Table
Can contain a variable length list of object pairs that can be indexed by a key.

```js
table = {}
```

```js
table = {
	Element = "Hello",
	OtherElement = "World"
}
```

```js
table = {}
table.Element = "Hello"
table.OtherElement = "World"
Text = table["Element"] + " " + table.OtherElement //Text = "Hello World"
```

## Default Operators
- [op_Equality(==)](./operators/Equality.md)
- [op_Inequality(!=)](./operators/Inequality.md)
- [op_ArrayAccess([key])](./operators/ArrayAccess.md)
- [op_MemberAccess(.key)](./operators/MemberAccess.md)
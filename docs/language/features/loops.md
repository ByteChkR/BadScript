# Loops

## Keywords
all versions of for and while loops support both `break` and `continue`

## while
```js
x = 1
while(x == 1)
{

}
```

## for

### Syntax
```js
for <var> = <start> while <end condition> step <expr>
{

}
```

### Example
```js
for i = 0 while i < 10 step 1
{

}
```

### Step Variable
Is not required. The Parser assumes `1`
```js
for i = 0 while i < 10
{

}
```

## for (short version)

### Example
```js
for i = 0 while< 10
{

}
```
Equvalent End Condition without shortend syntax `i < 10`

### Keywords

```
while= : operator ==
while! : operator !=
while< : operator <
while> : operator >
while<= : operator <=
while>= : operator >=
```

## foreach

### Syntax
```js
foreach <value> in <collection>
{

}
```

### Example
`foreach` loop on an array
```js
A = [] //Create new Array
A[0] = "Value1"
A[1] = "Value2"
foreach v in A
{

}
```

`foreach` loop on new array containing keys of table T
```js
T = {}
T["Key1"] = "Value1"
T["Key2"] = "Value2"
foreach key in keys(T)
{
	
}
```

`foreach` loop on the keys of the table, skips list allocation but does not allow removing of entries
```js
T = {}
T["Key1"] = "Value1"
T["Key2"] = "Value2"
foreach key in T
{
	
}
```

`foreach` loop on key and value pairs
```js
T = {}
T["Key1"] = "Value1"
T["Key2"] = "Value2"
foreach (key, value) in T
{
	
}
```
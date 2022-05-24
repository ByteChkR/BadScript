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

### Special Cases
The user may create custom objects that can be enumerated on.

`range` operator in a for loop
```js
foreach i in 1..4 //range 1(inclusive) to 4(inclusive)
{

}
```

`GetCurrent/MoveNext` Enumerator Methods can be used to allow custom objects to be enumerated on.
Those objects are one time use only unless their inner state is reset.
```js

class RangeEnumerator
{
    _from = 0
    _to = 0
    _current = 0
    function RangeEnumerator(!from, !to)
    {
        _from = from
        _to = to
        _current = _from - 1
    }

    function MoveNext()
    {
        _current += 1 //Increase current
        if(_current > _to)
        {
            return false
        }
        return true
    }
    
    function GetCurrent() => return _current;
}

foreach i in new RangeEnumerator(1, 4) //Create new range object
{

}

```

`GetEnumerator` Method can be used to create reusable enumerable objects by saving the initial state and constructing the enumerator in `GetEnumerator()`
```js


class Range
{
    _from = 0
    _to = 0
    function Range(!from, !to)
    {
        _from = from
        _to = to
    }

    function GetEnumerator() => return new RangeEnumerator(_from, _to);
}

r = new Range(1, 4)
foreach i in r //Calls GetEnumerator() implicitly
{

}

```
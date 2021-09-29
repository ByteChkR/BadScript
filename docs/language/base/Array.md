# Array
Can contain a variable length list of Objects that can be indexed(starting at 0).

```js
array = []
```

```js
array = [
	"element0",
	"element1",
	true,
	11
]
```

## Default Operators
- [op_Equality(==)](./operators/Equality.md)
- [op_Inequality(!=)](./operators/Inequality.md)
- [op_ArrayAccess([index])](./operators/ArrayAccess.md)
	- Requires `index` to be a number

## Implemented Functions

### add
Adds one or more objects to the array.

```js
array = []
array.add(true) //Adds true to the array
array.add(1, 2, 3, "Hello World", null) //Adds those elements to the array
```

#### Required Parameters
- obj0
	- Element that will be added

#### Optional Parameters
- obj1...
	- Additional Elements that will be added.

### clear
Clears all items from the list

```js
array = [ 1, true, null ]
array.clear() //Array is now empty
```

### contentEquals
Returns true if the specified array and this array have the same contents

```js
array1 = [
	1, 2, 3
]

array2 = [
	1, 2, 3
]

if(array1.contentEquals(array2))
{
	//Array Contents are equal
}

```

### remove
Removes the first occurence of one or more elements from the list that are equal to the ones specified.

```js
array = [1, 2, 3]
array.remove(2) //Array Content: [1, 3]
array.remove(1, 3) //Array Content: []
```

#### Required Parameters
- obj0
	- Element that will be removed

#### Optional Parameters
- obj1...
	- Additional Elements that will be removed.

### removeAt
Removes one or more elements at the specified indices from the list.

```js
array = [1, 2, 3]
array.removeAt(1) //Array Content: [1, 3]
array.removeAt(1, 0) //Array Content: []
```

#### Required Parameters
- obj0
	- Index of the Element that will be removed

#### Optional Parameters
- obj1...
	- Additional Indices of Elements that will be removed.


### reverse
Reverses the Order of the Elements in the list

```js
array = [1, 2, 3]
array.reverse() //New Array Order: [ 3, 2, 1 ]
```

### size
Returns the amount of elements in the list

```js
array = [1, 2, 3]
array.size() //Returns 3
```

### swap
Swaps two elements in the list.

```js
array = [1, 2, 3]
array.swap(0, 1) //New Array Order: [2, 1, 3]
```

#### Required Parameters
- idx1
	- Index of the first Element that will be swapped
- idx2
	- Index of the second Element that will be swapped
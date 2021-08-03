# Operators
List of Supported Operators

## add(+)

Adds two objects together
Example: `c = a + b`

### Notes
Adding has multiple implementations based on the types of the objects that are added

`<object> + <object> = <string>`

`<number> + <number> = <number>`

`<number> + <object> = <string>`

`<object> + <number> = <string>`

## subtract(-)
Subtract b from a
Example: `c = a - b`

## multiply(\*)
Multiply a with b
Example: `c = a * b`
## divide(/)
Divide a by b
Example: `c = a / b`
## modulo(%)
Modulus of a by b
Example: `c = a % b`

## equal(==)
Returns true if two objects are equal
Example: `c = a == b`
## unequal(!=)
Returns true if two objects are unequal
Example: `c = a != b`
## less than(<)
Returns true if a is smaller than b
Example: `c = a < b`
## greater than(>)
Returns true if a is greater than b
Example: `c = a > b`
## less or equal(<=)
Returns true if a is less or equal to b
Example: `c = a <= b`
## greater or equal(>=)
Returns true if a is greater or equal to b
Example: `c = a >= b`

## logical and(&&)
Returns true if a and b both are true
Example: `c = a && b`
## logical or(||)
Returns true if a or b are true
Example: `c = a || b`
## logical xor(^)
Returns true if a or b are true.
Returns false if a and b are true
Example: `c = a ^ b`
## logical not(!)
Returns true if a is false
Example: `c = !a`

## assignment(=)
Assigns the value on the right side to the property on the left side.
Example: `c = 1`

## member access(.)
Accesses the Properties of an Object
Example: `c = a.b`

## nullcheck(??)
```js
a = null
notNull = a ?? 1
```

## nullchecked property(?.)
```js
a = {}
value = a?.propertyName //Null if a does not have property "propertyName". Otherwise its the value of "a.propertyName"
```

# Self Assigning Operators
Self assigning versions of normal operators

## add and assign(+=)
Adds a and b and stores the result in a
Example: `a += b`
## subtract and assign(+=)
Adds a and b and stores the result in a
Example: `a -= b`
## divide and assign(+=)
Adds a and b and stores the result in a
Example: `a /= b`
## multiply and assign(+=)
Adds a and b and stores the result in a
Example: `a *= b`
## modulus and assign(+=)
Adds a and b and stores the result in a
Example: `a %= b`
## and and assign(&=)
Adds a and b and stores the result in a
Example: `a &= b`
## or and assign(|=)
Adds a and b and stores the result in a
Example: `a |= b`
## xor and assign(^=)
Adds a and b and stores the result in a
Example: `a ^= b`

# Prefix/Postfix Increment/Decrement

## Prefix Increment
Adds 1 to a and returns the result
Example: `c = ++a`

## Prefix Decrement
Subtracts 2 from a and returns the result
Example: `c = --a`

## Postfix Increment
Adds 1 to a and returns the original value of a
Example: `c = a++`

## Postfix Decrement
Subtracts 1 from a and returns the original value of a
Example: `c = a--`
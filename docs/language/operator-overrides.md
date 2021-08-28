# Operator Overrides

Operators can be overridden for table objects.
This can be used to implement custom behaviour for all operations that have the object on the left hand side of the operation.

## Map of Operators and their "named functions"
Left: The internal Name of the Operator
Right: The function name that the runtime will search for
```
"++_Pre": "op_PreIncrement",
"--_Pre" : "op_PreDecrement",
"++_Post" : "op_PostIncrement",
"--_Post" : "op_PostDecrement",
"+" : "op_Plus",
"-" : "op_Minus",
"*" : "op_Multiply",
"/" : "op_Divide",
"%" : "op_Modulus",
"+=" : "op_SelfPlus",
"-=" : "op_SelfMinus",
"*=" : "op_SelfMultiply",
"/=" : "op_SelfDivide",
"%=" : "op_SelfModulus",
"&&" : "op_And",
"||" : "op_Or",
"^" : "op_XOr",
"&=" : "op_SelfAnd",
"|=" : "op_SelfOr",
"^=" : "op_SelfXOr",
"==" : "op_Equals",
"!=" : "op_InEqual",
"<=" : "op_LessOrEqual",
">=" : "op_GreaterOrEqual",
"<" : "op_LessThan",
">" : "op_GreaterThan",
"!" : "op_Not",
"??" : "op_NullCheck",
"[]" : "op_ArrayAccess",
"." : "op_PropertyAccess",
"()" : "op_Invoke"
```

## Example
Adding the ability to concatenate two tables
```js
function concat(t1, t2, t3)
{
	//Optimization
	if(t1 != t3)
	{
		foreach (k, v) in t1
		{
			t3[k] = v
		}
	}

	//Optimization
	if(t2 != t3)
	{	
		foreach (k, v) in t2
		{
			t3[k] = v
		}
	}

	return t3
}

table = {
	op_Plus = function(r) => return concat(table, r, {}),
	op_SelfPlus = function(r) => return concat(table, r, table)
}

test = {
	a = 1,
	b = 2,
	c = null,
}

//Creates new Table without modifying the current table
newTable = table + test

//Modifies the Current Table and adds all values from test
table += test

```
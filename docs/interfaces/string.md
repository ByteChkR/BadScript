# String Interface

Interface Key: `string`

Provides Functionality to perform string manipulation

## Functions

### toNumber(str)
Converts the specified string into a number

### trim(str)
returns the trimmed version of the string

### trimStart(str)
returns the start-trimmed version of the string

### trimEnd(str)
returns the end-trimmed version of the string

### split(str, split0, split1, split2, ...)
returns the result of splitting `str` at every splitString

### substr(str, start)/substr(str, start, length)
returns the substring at the specified start with the specified length

### charAt(str, idx)
returns the character at index

### endsWith(str, end)
returns true if `str` ends with `end`

### startsWith(str, start)
returns true if `str` starts with `start`

### indexOf(str, searchStr)
returns the index of the first occurence of `searchStr`

### insert(str, idx, s)
Inserts `s` at the specified index in string `str`

### lastIndexOf(str, searchStr)
returns the index of the last occurence of `searchStr`

### remove(str, start, length)
removes the specified range of characters from `str`

### replace(str, old, new)
replaces every occurence of `old` with `new` in `str`

### toArray(str)
returns an array of characters

### toUpper(str)
converts all lowercase characters to uppercase

### toLower(str)
converts all uppercase characters to lowercase

### length(str)
returns the length of the string

### isWhiteSpace(str)
returns true if the string is whitespace only

### isLetter(str)
returns true if the string is letters only

### isDigit(str)
returns true if the string is digits only

### format(formatStr, arg0, arg1, ...)
returns the formatted string using C# `string.format()`


[List of All Interfaces](./Interfaces.md)
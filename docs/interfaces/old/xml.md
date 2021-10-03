# XML Interface

Interface Key: `xml`

Provides Functionality to parse and change XML Files

## Objects

### XML Node Object

#### Properties

##### parentNode
returns the parentNode

##### firstChild
returns the firstChild

##### lastChild
returns the lastChild

##### nextSibling
returns the next sibling in the parent node

##### previousSibling
returns the previous sibling in the parent node

##### childCount
returns the number of child nodes

##### value
gets or sets the value of the xml node

##### innerText
gets or sets the innerText of the xml node

##### name
returns the name of the xmlNode

##### hasChildNodes
returns true if the current node has any childnodes

#### Functions

##### findChild(name)
finds the child with the specified name

##### childAt(index)
returns the child at the specified index


### XML Document Object
Inherits all Properties of `XML Node Object`

#### Functions

##### toString()
Returns the Documents string representation


## Functions

### createDoc(str)
Creates a new XML Document with the specified contents


[List of All Interfaces](./../Interfaces.md)
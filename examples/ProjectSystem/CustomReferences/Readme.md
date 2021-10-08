# Project System Custom References Example

## The Custom Reference Resolver
Adding a custom reference resolver protocol
Add a `ReferenceResolvers` that will resolve the reference.
Inside the Reference Resolver Path `%Reference.Target%` and `%Reference.Path%` are available.
```json
"ReferenceResolvers": {
	"customref" :"%BS_EXEC% run --nologo -f ./tools/customReferenceResolver.bs -a %Reference.Path%"
}
```

## Referencing a File with a custom protocol

Custom Reference Protocols get matched by the URL Scheme of the Path variable.
For the `customref` resolver we dont need to specify a target as only the path matters.
```json
"References": [
	{
		"Target": "none",
		"Path": "customref://my-reference"
	}
]
```

Building the Project will now resolve the reference and include it into the build.

# Project System Reference Example

Referencing a project in another projects build config.

The Referencing Project needs to add an entry into the `References` inside the `build`-Target

```json
"References": [
	{
		"Path": "../Referenced/build-settings.json",
		"Target": "build"
	}
]
```

The Referencing Project is now building the `build`-Target of the referenced project and includes the output in its build.
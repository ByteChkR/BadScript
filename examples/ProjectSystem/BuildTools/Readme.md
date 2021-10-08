# Project System BuildTools Example

Using a BuildTool to automatically increase the version number on each build

This is done using by defining a PreEvent that executes the file `./tools/increment_build.bs`. Which applies the calender versioning scheme to the project.

```json
"PreEvents": [
	"%BS_EXEC% run --nologo -f ./tools/increment_build.bs"
]
```

The Version now gets incremented with calender versioning for each build.
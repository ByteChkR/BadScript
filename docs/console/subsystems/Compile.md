# Bad Script Console `compile` Subsystem

Runs one or multiple scripts from source or a serialized scripts.

## Required Arguments
- `-i/--input [inputFile]`
	- Specifies a script that will be serialized.
- `-o/--output [outputFile]`
	- Specifies the output path of the serialized script.

## Optional Arguments
- `--include [dir0] [dir1] ...`
	- Sets the include directories that get loaded prior to the script serialization
	- Default Value: `[BS_DIR]/data/include`
- `--interfaces [interfaceName0] [interfaceName1] ...`
	- Sets the Active interfaces that are loaded prior to the script serialization
	- Default Value: `Convert, Console, Collection`
- `--optimize`
	- If set, the runtime will optimize all loaded scripts before serialization
	- Default Value: `false`
- `--nologo`
	- Does not display Console Header information when set.

## Examples

Serialize the Script `MyScript.bs` with all default values.
```
bs compile -i MyScript.bs -o MyScript.bsc
```

___

[List of all Console Subsystems](./Subsystems.md)
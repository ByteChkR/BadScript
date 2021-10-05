# Bad Script Console `run` Subsystem

Runs one or multiple scripts from source or a serialized scripts.

## Required Arguments
- `-f/--files [file0] [file1] ...`
	- Specifies the scripts or directories that will be executed.

## Optional Arguments
- `-a/--args [arg0] [arg1] ...`
	- Specifies the start arguments passed to the script.
- `-b/--benchmark [True|False]`
	- If set to true, the runtime will measure and print the time it took to execute the scripts
- `--include [dir0] [dir1] ...`
	- Sets the include directories that get loaded prior to the script execution
	- Default Value: `[BS_DIR]/data/include`
- `--interfaces [interfaceName0] [interfaceName1] ...`
	- Sets the Active interfaces that are loaded prior to the script execution
	- Default Value: `Convert, Console, Collection`
- `--optimize`
	- If set, the runtime will optimize all loaded scripts before execution
	- Default Value: `false`
- `--nologo`
	- Does not display Console Header information when set.

## Examples

Execute the Script `MyScript.bs` with all default values.
```
bs run -f MyScript.bs
```

Execute the Script `MyScript.bs` with startup arguments.
```
bs run -f MyScript.bs -a Argument0 Argument1
```

Execute the Script `MyScript.bs` with the [`FileSystem`](../../interfaces/new/FileSystem.bs) Interface loaded by default.
```
bs run -f MyScript.bs --interfaces Convert Console Collection FileSystem
```
Note: When Explicitly specifying the interfaces, the default interfaces need to be specified.

___

[List of all Console Subsystems](./Subsystems.md)
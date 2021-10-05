# Bad Script Console `app` Subsystem

Runs an app package

## Required Arguments
- `Input App`
	- Specifies a app that will be executed.

## Optional Arguments
- `-a/--args [arg0] [arg1] ...`
	- Specifies the start arguments passed to the app.
- `--nologo`
	- Does not display Console Header information when set.

## Examples

Execute the App `MyApp.bsapp` with all default values.
```
bs app MyApp.bsapp
```

Execute the App `MyApp.bsapp` with startup arguments.
```
bs app MyApp.bsapp -a Argument0 Argument1
```

___

[List of all Console Subsystems](./Subsystems.md)